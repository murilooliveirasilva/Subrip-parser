using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMiners.Bussiness.Interfaces;
using MMiners.Bussiness.Models;
using MMiners.Bussiness.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MM.Test
{

    [TestClass]
    public class SubtituloTest
    {
        private readonly ISubtituloRepository _subtituloRepository;
        private SubtituloService _subtituloService;

        private string caminhoTestesPadrao;
        
        public SubtituloTest()
        {
            string startupPath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
            var pos = pathItems.Reverse().ToList().FindIndex(x => string.Equals("bin", x));
            string projectPath = String.Join(Path.DirectorySeparatorChar.ToString(), pathItems.Take(pathItems.Length - pos - 1));
            caminhoTestesPadrao = Path.Combine(projectPath, "ArquivosTest");
            _subtituloService = new SubtituloService(_subtituloRepository);
        }

        /*
        bool ValidarSeArquivoExiste(string caminhoArquivo);
        bool ValidarExtensaoArquivo(string caminhoArquivo);
        bool Validarsubtitulo(string arquivo, out List<Subtitulo> ls);
        byte[] CriarSubtituloAjustado(List<Subtitulo> lsSubtitulo, int incrementoSegundosIniciais, int incrementoSegundosFinais);
        string CriarPastas(string nome, string path);
        */

        [TestMethod]
        public void TestValidarSeArquivoExisteExists()
        {
            string arquivo = Path.Combine(caminhoTestesPadrao, "Ex1_Ok.srt");
            bool sucesso = _subtituloService.ValidarSeArquivoExiste(arquivo);
            Assert.IsTrue(sucesso);
        }

        [TestMethod]
        public void TestValidarExtensaoArquivoNotExists()
        {
            string arquivo = Path.Combine(caminhoTestesPadrao, "Ex1_Ok_asda.srt");
            bool sucesso = _subtituloService.ValidarSeArquivoExiste(arquivo);
            Assert.IsFalse(sucesso);
        }

        [TestMethod]
        public void TestValidarExtensaoArquivoExist()
        {
            string arquivo = Path.Combine(caminhoTestesPadrao, "Ex1_Ok.srt");
            bool sucesso = _subtituloService.ValidarExtensaoArquivo(arquivo);
            Assert.IsTrue(sucesso);
        }

        [TestMethod]
        public void TestValidarExtensaoArquivoNotExist()
        {
            string arquivo = Path.Combine(caminhoTestesPadrao, "Ex1_Ok.srt1");
            bool sucesso = _subtituloService.ValidarExtensaoArquivo(arquivo);
            Assert.IsFalse(sucesso);
        }


        [TestMethod]
        public void TestValidarsubtituloExists()
        {
            string arquivo = Path.Combine(caminhoTestesPadrao, "Ex1_Ok.srt");
            List<Subtitulo> ls = new List<Subtitulo>();
            string caminhoArquivo = System.IO.File.ReadAllText(arquivo, System.Text.Encoding.UTF8);

            bool sucesso = _subtituloService.Validarsubtitulo(caminhoArquivo, out ls);
            Assert.IsTrue(sucesso);
        }

        [TestMethod]
        public void TestValidarsubtituloNotExists()
        {
            string arquivo = Path.Combine(caminhoTestesPadrao, "Ex4_Error.srt");
            List<Subtitulo> ls = new List<Subtitulo>();
            bool sucesso = _subtituloService.Validarsubtitulo(arquivo, out ls);
            Assert.IsFalse(sucesso);
        }


        [TestMethod]
        public void TestCriarSubtituloAjustadoExists()
        {
            string arquivo = Path.Combine(caminhoTestesPadrao, "Ex1_Ok.srt");
            List<Subtitulo> ls = new List<Subtitulo>();
            _subtituloService.Validarsubtitulo(arquivo, out ls);

            bool sucesso = _subtituloService.CriarSubtituloAjustado(ls,1,1.5).Length > 0;
            Assert.IsTrue(sucesso);
        }

        [TestMethod]
        public void TesCriarPastasExists()
        {

            bool sucesso = _subtituloService.CriarPastas(caminhoTestesPadrao, "Ex1_Ok.srt") != "";



            Assert.IsTrue(sucesso);
        }

    }
}
