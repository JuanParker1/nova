using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class ClientCondFin
    {
        public int Frais_Dossier_import { get; set; }
        public int Frais_Dossier_export { get; set; }
        public int Surestarie_import { get; set; }
        public int Surestarie_export { get; set; } 
        public int Detention_import { get; set; }
        public int Detention_export { get; set; }
        public int Reparation_import { get; set; }
        public int Valeur_residuel_import { get; set; }
        public int Reparation_export { get; set; }
        public int Valeur_residuel_export { get; set; } 
        public int Operation_divers_import { get; set; }
        public int Operation_divers_export { get; set; }
        public int fret_import{ get; set; }
        public int fret_export { get; set; }

        public string Caution_type_import { get; set; }
        public int caution_montant_import { get; set; } 
        public string Caution_moyen_payement_import { get; set; }
        public string Caution_moyen_payement_export { get; set; }
        public int Caution_montant_export { get; set; }

        public int Veh_manut_import { get; set; }
        public int Veh_manut_export { get; set; }
        public int Veh_pg_import { get; set; }
        public int Veh_pg_export { get; set; }
        public int Veh_fret_import { get; set; }
        public int Veh_fret_export { get; set; }
        public int Veh_sejour_import { get; set; }
        public int Veh_sejour_export { get; set; }
        public int Veh_od_export { get; set; }
        public int Veh_od_import { get; set; }

        public int Conv_manut_import { get; set; }
        public int Conv_manut_export { get; set; }
        public int Conv_od_import { get; set; }
        public int Conv_od_export { get; set; }
        public int Conv_sejour_import { get; set; }
        public int Conv_sejour_export { get; set; }

        public int Plafond_import { get; set; }
        public int Plafond_export { get; set; }

        public int Covvd { get; set; } 
    }
}
