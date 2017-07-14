using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syslaps.Pdv.Core;
using Syslaps.Pdv.Core.Dominio;
using Syslaps.Pdv.Core.Dominio.Base;

namespace Syslaps.Pdv.Test
{
    [TestClass]
    public class Geral
    {
        [TestMethod]
        public void DevoPoderIniciarAAplicacao()
        {
            var bootstrap = ContainerIoc.GetInstance<Bootstrap>();
            bootstrap.IniciarProcessoCompleto();
            Assert.IsTrue(bootstrap.Status == EnumStatusDoResultado.MensagemDeSucesso);
        }


    }
}
