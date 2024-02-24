using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCC_DataEval_App
{
    public class MvpFactory
    {
        public static RawDataView GetRawDataView()
        {
            IRawDataModel model = new DataModel();
            RawDataView view = new RawDataView();
            RawDataPresenter presenter = new RawDataPresenter(view, model);
            view.Presenter = presenter;
            return view;
        }
        
    }
}
