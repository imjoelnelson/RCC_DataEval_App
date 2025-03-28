using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public class PcaForQc
    {
        /// <summary>
        /// Loadings for first three PCs; item1 = targetID; item2 = double[4] containing the loadings for the 1st 4 PCs
        /// </summary>
        public Tuple<string, double[]>[] PcLoadings { get; set; }

        public double[] EigenVals { get; set; }
        public double[][] EigenVects { get; set; }
        
        public PcaForQc(Tuple<int, int, double>[] normalizedData, List<Rcc> rccs)
        {
            // Convert NormData to a data matrix; rows = samples, columns = targets
            int[] sampleIDs;
            string[] geneNames;

            double[][] dat = GetMatrix(normalizedData, out sampleIDs, out geneNames);

            if(dat.Any(x => x.Length != dat[0].Length))
            {
                throw new Exception("Normalized Data table cannot be a jagged array; The number of targets differs for one or more samples");
            }

            // Convert counts to Z-scores
            double[][] zScores = GetZScore(dat);

            // Calculate symatrical covar matrix
            double[][] covar = GetCovarMatrix(zScores, false);

            double[] eval;
            double[][] evec;

            // Calculate eigenvectors and eigenvalues
            Eigen(covar, out eval, out evec);
            EigenVals = eval;
            EigenVects = evec;
        }

        /// <summary>
        /// Creates a matrix from collection of normalized data tuples
        /// </summary>
        /// <param name="normalizedData">List of tuples containing normalized data; item1 = target name, item2 = sampleID, item3 = normalizzed count</param>
        /// <param name="sampleIDs">The sampleID row names of the new matrix</param>
        /// <param name="geneNames">The target name column </param>
        /// <returns>A double[][] of normalized counts with rows being samples and columns being targets/genes</returns>
        private double[][] GetMatrix(Tuple<int, int, double>[] normalizedData, out int[] sampleIDs, out string[] geneNames)
        {
            // Convert NormData to a data matrix; rows = samples, columns = targets; each row ordered by gene names
            int[] _sampleIDs = normalizedData.Select(x => x.Item2).Distinct().ToArray();
            IEnumerable<string> _geneNames = normalizedData.Select(x => x.Item1).Distinct();

            double[][] retVal = new double[_sampleIDs.Length][];
            for (int i = 0; i < _sampleIDs.Length; i++)
            {
                var temp = normalizedData.Where(x => x.Item2 == _sampleIDs[i] && _geneNames.Contains(x.Item1))
                                         .OrderBy(x => x.Item1)
                                         .Select(x => x.Item3)
                                         .ToArray();
                retVal[i] = temp;
            }

            // 
            sampleIDs = _sampleIDs;
            geneNames = _geneNames.OrderBy(x => x).ToArray();

            return retVal;
        }

        /// <summary>
        /// Calculates column-based Z-score from a retangular matrix of doubles; means and sds calculated on columns of the matrix
        /// </summary>
        /// <param name="dat">The data matrix to convert</param>
        /// <returns></returns>
        private double[][] GetZScore(double[][] dat)
        {
            if(dat.Any(x => x.Length != dat[0].Length))
            {
                throw new Exception("Zscore calculation argument exception: data matrix cannot be a jagged array");
            }

            double[][] mat1 = new double[dat.Length][];
            for (int i = 0; i < dat.Length; i++)
            {
                double mean = dat[i].Average();
                double sd = Statistics.StandardDeviation(dat[i]);
                mat1[i] = dat[i].Select(x => (mean - x) / sd).ToArray();
            }

            return mat1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dat">data matrix to get covar from; z-score normalized</param>
        /// <param name="transformFirst">bool indicating whether to rotate matrix 90 degrees</param>
        /// <returns>Symmetrical square covar matrix</returns>
        private double[][] GetCovarMatrix(double[][] dat, bool transformFirst)
        {
            // Transform if indicated
            double[][] tempMat;
            if(transformFirst)
            {
                tempMat = Util.TransformTable(dat);
            }
            else
            {
                tempMat = dat;
            }

            // Initialize empty square matrix
            double[][] retVal = new double[tempMat.Length][];
            for(int i = 0; i < tempMat.Length; i++)
            {
                retVal[i] = new double[tempMat.Length];
            }

            // Calculate covariances
            for(int r = 0; r < tempMat.Length; r++)
            {
                for (int c = r; c < dat.Length; c++)
                {
                    double rMean = tempMat[r].Average();
                    double cMean = tempMat[c].Average();
                    double collector = 0;
                    for (int i = 0; i < tempMat[0].Length; i++)
                    {
                        collector += (tempMat[r][i] - rMean) * (tempMat[c][i] - cMean);
                    }
                    retVal[r][c] = retVal[c][r] = collector;
                }
            }

            return retVal;
        }

        static void Eigen(double[][] A,
            out double[] eval, out double[][] evec)
        {
            // Jacobi algorithm based on GSL implementation
            // assumes A is square symmetric
            // OK when applied to covariance matrix
            int m = A.Length; int n = A[0].Length;
            // if m != n throw an exception
            int maxRot = 100 * m * m;  // heuristic
            double redSum = 0.0;

            eval = new double[m];
            evec = MatIdentity(m);

            for (int i = 0; i < maxRot; ++i)
            {
                double nrm = Normalize(A);

                if (nrm == 0.0)  // mildly risky
                    break;

                for (int p = 0; p < n; ++p)
                {
                    for (int q = p + 1; q < n; ++q)
                    {
                        double c; double s;
                        redSum += Symschur2(A, p, q, out c, out s);
                        // Compute A := J^T A J 
                        Apply_Jacobi_L(A, p, q, c, s);
                        Apply_Jacobi_R(A, p, q, c, s);
                        // Compute V := V J 
                        Apply_Jacobi_R(evec, p, q, c, s);
                    }
                }
            }

            // nrot = i;
            for (int p = 0; p < n; ++p)
            {
                double ep = A[p][p];
                eval[p] = ep;
            }
        }

        static double[][] MatIdentity(int n)
        {
            double[][] result = MatCreate(n, n);
            for (int i = 0; i < n; ++i)
                result[i][i] = 1.0;
            return result;
        }

        static double[][] MatCreate(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }

        static double Normalize(double[][] A)
        {
            int m = A.Length; int n = A[0].Length;
            double scale = 0.0;
            double ssq = 1.0;

            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j < n; j++)
                {
                    double Aij = A[i][j];

                    // compute norm of off-diagonal elements
                    if (i == j) continue;

                    if (Aij != 0.0)
                    {
                        double ax = Math.Abs(Aij);

                        if (scale < ax)
                        {
                            ssq =
                              1.0 + ssq * (scale / ax) * (scale / ax);
                            scale = ax;
                        }
                        else
                        {
                            ssq += (ax / scale) * (ax / scale);
                        }
                    }

                }
            }

            double sum = scale * Math.Sqrt(ssq);
            return sum;
        }

        static double Symschur2(double[][] A, int p, int q,
               out double c, out double s)
        {
            // Symmetric Schur decomposition 2x2 matrix
            double Apq = A[p][q];
            if (Apq != 0.0)
            {
                double App = A[p][p];
                double Aqq = A[q][q];
                double tau = (Aqq - App) / (2.0 * Apq);
                double t, c1;

                if (tau >= 0.0)
                {
                    t = 1.0 / (tau + Hypot(1.0, tau));
                }
                else
                {
                    t = -1.0 / (-tau + Hypot(1.0, tau));
                }

                c1 = 1.0 / Hypot(1.0, t);
                c = c1; s = t * c1;
            }
            else  // Apq == 0.0
            {
                c = 1.0; s = 0.0;
            }

            return Math.Abs(Apq);
        }

        static double Hypot(double x, double y)
        {
            // fancy sqrt(x^2 + y^2)
            double xabs = Math.Abs(x);
            double yabs = Math.Abs(y);
            double min, max;

            if (xabs < yabs)
            {
                min = xabs; max = yabs;
            }
            else
            {
                min = yabs; max = xabs;
            }

            if (min == 0)
                return max;

            double u = min / max;
            return max * Math.Sqrt(1 + u * u);
        }

        static void Apply_Jacobi_L(double[][] A, int p,
            int q, double c, double s)
        {
            int n = A[0].Length;

            // Apply rotation to matrix A,  A' = J^T A 
            for (int j = 0; j < n; ++j)
            {
                double Apj = A[p][j];
                double Aqj = A[q][j];
                A[p][j] = Apj * c - Aqj * s;
                A[q][j] = Apj * s + Aqj * c;
            }
        }

        static void Apply_Jacobi_R(double[][] A, int p,
            int q, double c, double s)
        {
            int m = A.Length;

            // Apply rotation to matrix A,  A' = A J 
            for (int i = 0; i < m; ++i)
      {
                double Aip = A[i][p];
                double Aiq = A[i][q];
                A[i][p] = Aip * c - Aiq * s;
                A[i][q] = Aip * s + Aiq * c;
            }
        }
    }
}