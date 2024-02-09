using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCC_DataEval_App
{
    public class PkcReader : IPkcReader
    {
        public string Name { get; private set; }
        public Dictionary<string, ProbeItem> DspTranslator { get; private set; }


        public PkcReader(string filePath)
        {
            Name = Path.GetFileNameWithoutExtension(filePath);

            string fullRead;
            try
            {
                fullRead = File.ReadAllText(filePath);
            }
            catch (Exception er)
            {
                MessageBox.Show($"{er.Message}\r\n\r\n{er.StackTrace}", "PKC Read Error", MessageBoxButtons.OK);
                fullRead = string.Empty;
                return;
            }
            JObject json = JObject.Parse(fullRead);
            JArray targetArray = (JArray)json["Targets"];
            Dictionary<string, ProbeItem> translator = new Dictionary<string, ProbeItem>(targetArray.Count * 8);
            string[] lets = new string[] { "A", "B", "C", "D", "E", "F", "G" };
            for (int i = 0; i < targetArray.Count; i++)
            {
                List<string> ids = targetArray[i]["DSP_ID"].Select(x => ((string)x).Split(new string[] { ": " }, StringSplitOptions.None)[1]).ToList();
                for (int j = 0; j < ids.Count; j++)
                {
                    translator.Add(ids[j], new ProbeItem((string)targetArray[i]["CodeClass"],
                                                         (string)targetArray[i]["DisplayName"],
                                                         lets[j],
                                                         RlfType.DSP));
                }
            }

            DspTranslator = translator;
        }
    }
}
