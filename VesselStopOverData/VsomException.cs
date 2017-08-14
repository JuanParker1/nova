using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    class VsomException
    {
    }


    [Serializable]
    public class EscaleException : ApplicationException
    {
        public EscaleException()
            : base() { }

        public EscaleException(string message)
            : base(message) { }
    }

    [Serializable]
    public class ClientException : ApplicationException
    {
        public ClientException()
            : base() { }

        public ClientException(string message)
            : base(message) { }
    }

    [Serializable]
    public class ConnaissementException : ApplicationException
    {
        public ConnaissementException()
            : base() { }

        public ConnaissementException(string message)
            : base(message) { }
    }

    [Serializable]
    public class ConnexionException : ApplicationException
    {
        public ConnexionException()
            : base() { }

        public ConnexionException(string message)
            : base(message) { }
    }

    [Serializable]
    public class EnregistrementInexistant : ApplicationException
    {
        public EnregistrementInexistant()
            : base() { }

        public EnregistrementInexistant(string message)
            : base(message) { }
    }

    [Serializable]
    public class ManifesteException : ApplicationException
    {
        public ManifesteException()
            : base() { }

        public ManifesteException(string message)
            : base(message) { }
    }

    [Serializable]
    public class IdentificationException : ApplicationException
    {
        public IdentificationException()
            : base() { }

        public IdentificationException(string message)
            : base(message) { }
    }

    [Serializable]
    public class FacturationException : ApplicationException
    {
        public FacturationException()
            : base() { }

        public FacturationException(string message)
            : base(message) { }
    }

    [Serializable]
    public class CubageException : ApplicationException
    {
        public CubageException()
            : base() { }

        public CubageException(string message)
            : base(message) { }
    }

    [Serializable]
    public class TransfertSortieException : ApplicationException
    {
        public TransfertSortieException()
            : base() { }

        public TransfertSortieException(string message)
            : base(message) { }
    }

    [Serializable]
    public class HabilitationException : ApplicationException
    {
        public HabilitationException()
            : base() { }

        public HabilitationException(string message)
            : base(message) { }
    }
}
