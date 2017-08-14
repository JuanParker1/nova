using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using VesselStopOverData;

namespace VesselStopOverPresentation
{
    public class VehiculeCubedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Data.Linq.EntitySet<CUBAGE_VEHICULE> cubs = value as System.Data.Linq.EntitySet<CUBAGE_VEHICULE>;
            return cubs.Count(cub => !cub.DateVal.HasValue) != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VehiculePositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            VsomParameters vsp = new VsomParameters();

            int idVeh = System.Convert.ToInt32(value);

            VEHICULE veh = vsp.GetVehiculeByIdVeh(idVeh);

            if (!veh.IdVehAP.HasValue)
            {
                List<OCCUPATION> occupations = veh.OCCUPATION.Where(occ => !occ.DateFin.HasValue).ToList<OCCUPATION>();
                if (occupations.Count != 0)
                {
                    return occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.PARC.NomParc + " : " + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.LigEmpl + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.ColEmpl;
                }
                else
                {
                    return "Non parqué";
                }
            }
            else
            {
                return Convert(veh.IdVehAP, targetType, parameter, culture);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConteneurPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            VSOMAccessors vsomAcc = new VSOMAccessors();
             // VsomParameters vsp = new VsomParameters();
            int idCtr = System.Convert.ToInt32(value);

            CONTENEUR ctr = vsomAcc.GetConteneurByIdCtr(idCtr);

            List<OCCUPATION> occupations = ctr.OCCUPATION.Where(occ => !occ.DateFin.HasValue).ToList<OCCUPATION>();
            if (occupations.Count != 0)
            {
                return occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.PARC.NomParc + " : " + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.LigEmpl + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.ColEmpl;
            }
            else
            {
                return "Non parqué";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateDebarquementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            VSOMAccessors vsomAcc = new VSOMAccessors();
              //VsomParameters vsp = new VsomParameters();
            int idCtr = System.Convert.ToInt32(value);

            CONTENEUR ctr = vsomAcc.GetConteneurByIdCtr(idCtr);

            return ctr.OPERATION_CONTENEUR.FirstOrDefault<OPERATION_CONTENEUR>(op => op.IdTypeOp == 12).DateOp.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateEmbarquementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            VSOMAccessors vsomAcc = new VSOMAccessors();
              //VsomParameters vsp = new VsomParameters();
            int idCtr = System.Convert.ToInt32(value);

            CONTENEUR ctr = vsomAcc.GetConteneurByIdCtr(idCtr);

            return ctr.OPERATION_CONTENEUR.FirstOrDefault<OPERATION_CONTENEUR>(op => op.IdTypeOp == 283).DateOp.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MontantOSConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Data.Linq.EntitySet<LIGNE_SERVICE> lignes = value as System.Data.Linq.EntitySet<LIGNE_SERVICE>;
            return lignes.Sum(lg => lg.PULS.Value * lg.QLS.Value * ((lg.CodeTVA == "TVAAP" || lg.CodeTVA=="TVATI") ? 1.1925f : 1));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class QteLivreeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value == 0)
            {
                return 0;
            }
            else
            {
                VsomParameters vsp = new  VsomParameters();
                int idDisposition = System.Convert.ToInt32(value);
                DISPOSITION_CONTENEUR dispositionCtr = vsp.GetMiseDispositionByIdDisposition(idDisposition);

                return dispositionCtr.CONTENEUR_TC.Count;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class QteRestLivrerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value == 0)
            {
                return 0;
            }
            else
            {
                VsomParameters vsp = new  VsomParameters();
                int idDisposition = System.Convert.ToInt32(value);
                DISPOSITION_CONTENEUR dispositionCtr = vsp.GetMiseDispositionByIdDisposition(idDisposition);

                return dispositionCtr.NombreTC - dispositionCtr.CONTENEUR_TC.Count;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConteneurStationnementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();
            int idCtr = System.Convert.ToInt32(value);

            CONTENEUR ctr = vsomAcc.GetConteneurByIdCtr(idCtr);

            if (ctr.FFCtr.HasValue)
            {
                if (!ctr.DSCtr.HasValue)
                {
                    return (DateTime.Now.Date - ctr.FFCtr.Value.Date).Days + " Jour(s)";
                }
                else
                {
                    return "Sorti";
                }
            }
            else
            {
                return "N/A";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StationnementDepasseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            VSOMAccessors vsomAcc = new VSOMAccessors();
     //VsomParameters vsp = new VsomParameters();
            int idCtr = System.Convert.ToInt32(value);

            CONTENEUR ctr = vsomAcc.GetConteneurByIdCtr(idCtr);

            if (ctr.FFCtr.HasValue)
            {
                if (!ctr.DSCtr.HasValue && (DateTime.Now.Date - ctr.FFCtr.Value.Date).Days > 30)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MafiStationnementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            VsomParameters vsp = new VsomParameters();

            int idMafi = System.Convert.ToInt32(value);

            MAFI ctr = vsp.GetMafiImportByIdMafi(idMafi);

            if (ctr.FFMafi.HasValue)
            {
                if (!ctr.DSMafi.HasValue)
                {
                    return (DateTime.Now.Date - ctr.FFMafi.Value.Date).Days + " Jour(s)";
                }
                else
                {
                    return "Sorti";
                }
            }
            else
            {
                return "N/A";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
