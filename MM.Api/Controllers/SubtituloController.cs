using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MM.Data;
using MMiners.Bussiness.Interfaces;
using MMiners.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MMiners.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubtituloController : Controller
    {
        private readonly ISubtituloService _subtituloService;
        private readonly ISubtituloRepository _subtituloRepository;

        public SubtituloController( ISubtituloRepository subtituloRepository,
                                    ISubtituloService subtituloService)
        {
            _subtituloService = subtituloService;
            _subtituloRepository = subtituloRepository;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<string> Upload(IFormFile arquivo)
        {
            if(arquivo != null && arquivo.Length>0)
            {
                try
                {
                    string nome = arquivo.FileName.ToLower();
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Arquivos");

                    string caminhoArquivo = _subtituloService.CriarPastas(nome, path);

                    bool arquivoValido = _subtituloService.ValidarExtensaoArquivo(caminhoArquivo) &&
                                         !_subtituloService.ValidarSeArquivoExiste(caminhoArquivo);

                    if (arquivoValido)
                    {
                        using (FileStream FS = new FileStream(caminhoArquivo, FileMode.Create))
                        {
                            await arquivo.CopyToAsync(FS);
                        }
                        return $"Upload do arquivo {nome} feito com sucesso";
                    }
                    return $"Falha no upload do arquivo {nome}";

                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }

            throw new ArgumentException("Upload falhou");
        }

        [HttpPost]
        [Route("parse")]
        public async Task<List<Subtitulo>> Parse(string nomeArquivo)//, DateTimeOffset dtInicio, DateTimeOffset dtFim)
        {
            if (nomeArquivo == null || nomeArquivo.Length ==0)
                throw new ArgumentException("Arquivo selecionado não existe.Insira um arquivo válido");

            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Arquivos");

                string caminhoArquivo = _subtituloService.CriarPastas(nomeArquivo.ToLower().Trim(), path);              

                bool arquivoExiste = _subtituloService.ValidarSeArquivoExiste(caminhoArquivo);
                if (!arquivoExiste)
                    throw new ArgumentException("Arquivo selecionado não existe.Insira um arquivo válido");

                bool extensaoValida = _subtituloService.ValidarSeArquivoExiste(caminhoArquivo);
                if(!extensaoValida)
                    throw new ArgumentException("Extensão do arquivo incorreta");

                string arquivo = await System.IO.File.ReadAllTextAsync(caminhoArquivo, System.Text.Encoding.UTF8);

                List<Subtitulo> lsSubtitulos = new List<Subtitulo>();
                bool validarSubtitulo = _subtituloService.Validarsubtitulo(arquivo, out lsSubtitulos);

                if (validarSubtitulo)
                    return lsSubtitulos;

                return new List<Subtitulo>();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("ajustartimecode")]
        public async Task<FileContentResult> AjustarTimeCode(string nomeArquivo, string secInicial, string secFinal)
        {
            if (nomeArquivo == null || nomeArquivo.Length == 0)
                throw new ArgumentException("Arquivo selecionado não existe. Insira um arquivo válido");

            secInicial = secInicial.Replace('.', ',');
            secFinal   = secFinal.Replace('.', ',');

            double incrementoSegundosIniciais, incrementoSegundosFinais;
            bool secInicialEhValido = double.TryParse(secInicial, out incrementoSegundosIniciais);
            bool secFinalEhValido = double.TryParse(secFinal, out incrementoSegundosFinais);

            try
            {
                if (secInicialEhValido && secFinalEhValido)
                {
                    var lsSubtitulo = await Parse(nomeArquivo);

                    if(lsSubtitulo == null || lsSubtitulo.Count == 0)
                        throw new ArgumentException("Arquivo selecionado em branco.Insira um arquivo válido");

                    byte[] arBytesSubtitulo = _subtituloService.CriarSubtituloAjustado(lsSubtitulo, incrementoSegundosIniciais, incrementoSegundosFinais);

                    if(arBytesSubtitulo != null && arBytesSubtitulo.Length>0)
                        return File(arBytesSubtitulo, "application/octet-stream", "Download.srt");
                    throw new ArgumentException("Arquivo invalido");

                }
                throw new ArgumentException("Timecode inválido");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public IEnumerable<ArquivosSubtituloDTO> Get()
        {
            try
            {
                string path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "Arquivos");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                DirectoryInfo dir = new System.IO.DirectoryInfo(path);
                var arquivos = dir.GetFiles("*.srt");

                return arquivos.Select((ArquivosSubtituloDTO, index) => 
                    new ArquivosSubtituloDTO
                    {
                        id = index,
                        arquivo = ArquivosSubtituloDTO.Name
                    }).ToList();
            }
            catch(Exception)
            {
                throw new ArgumentException("Erro ao listar arquivos");
            }
        }


    }
}
