using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
   public class PwdHash
    {
        /// <summary>
        /// represente le premier caractère ascii quon puisse utiiser dans le cryptage
        /// </summary>
        private int startingASCII = 32;
        /// <summary>
        /// represente le last caractère ascii quon puisse utiliser
        /// </summary>
        private int endingASCII = 125;
        /// <summary>
        /// represent le mot a crypter
        /// </summary>
        private string _word;

        public PwdHash(string word)
        {
            _word = word;
        }



        /// <summary>
        /// crypte le mot envoie
        /// </summary>
        /// <returns></returns>
        public string Encrypt()
        {
            /*la regle de cryptage consiste a remplacer chaque code ascii par celui qui le corespond 
             * en fonction de sa position (<79 ou >79)
             * si <79 il est remplace par 33+postion
             * si >79 il est remplacer par 125-position
             */
            byte[] sbyt = System.Text.Encoding.ASCII.GetBytes(_word);
            string progressdata = string.Empty;
            int temp; int ftemp; char schar;
            for (int i = 0; i < sbyt.Length; i++)
            {
                if (sbyt[i] > 79)
                {
                    temp = int.Parse(sbyt[i].ToString()) - 79;
                    ftemp = 33 + temp;
                    //cas fait pour le caractère ' (39) qui pourrait empecher lexecution de la requete upd. il est remplacer par 139
                    /* if (ftemp == 39) 
                         ftemp = 160;
                     */
                    schar = (char)(ftemp);
                    progressdata = string.Format("{0}{1}", progressdata, schar);
                }
                if (sbyt[i] < 79)
                {
                    temp = 79 - int.Parse(sbyt[i].ToString());
                    //if (temp > 126) temp = int.Parse(sbyt[i].ToString());
                    progressdata = string.Format("{0}{1}", progressdata, (char)(125 - temp));
                }

                if (sbyt[i] == 79)
                {
                    progressdata = string.Format("{0}O", progressdata);
                }

            }
            
            return progressdata;
            //return _word;
        }

        /// <summary>
        /// decrypte le mot actuelle
        /// </summary>
        /// <returns></returns>
        public string Decrypt()
        {
            byte[] sbyt = System.Text.Encoding.ASCII.GetBytes(_word);
            string progressdata = string.Empty;
            int temp;
            for (int i = 0; i < sbyt.Length; i++)
            {
                if (sbyt[i] > 79)
                {
                    if (sbyt[i] == 160)//cas exceptionnel du caractère ' 
                    {
                        temp = int.Parse(sbyt[i].ToString()) - 33;
                        //if (temp > 126) temp = int.Parse(sbyt[i].ToString());
                        progressdata = string.Format("{0}{1}", progressdata, (char)(39));
                    }
                    else
                    {
                        temp = 125 - int.Parse(sbyt[i].ToString());
                        //if (temp < 33) temp = int.Parse(sbyt[i].ToString());
                        progressdata = string.Format("{0}{1}", progressdata, (char)(79 - temp));
                    }
                }
                if (sbyt[i] < 79)
                {
                    temp = int.Parse(sbyt[i].ToString()) - 33;
                    //if (temp > 126) temp = int.Parse(sbyt[i].ToString());
                    progressdata = string.Format("{0}{1}", progressdata, (char)(79 + temp));
                }

                if (sbyt[i] == 79)
                {
                    progressdata = string.Format("{0}O", progressdata);
                }
            }
            return progressdata;
        }
        
        /// <summary>
        /// renvoie le mot actuel
        /// </summary>
        /// <returns></returns>
        public string Current()
        {
            return _word;
        }


    }

}
