using MMiners.Bussiness.Interfaces;
using MMiners.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMiners.Bussiness.Services
{
    public class SubtituloService: ISubtituloService
    {
        private readonly ISubtituloRepository _subtituloRepository;

        public SubtituloService(ISubtituloRepository subtituloRepository)                           
        {
            _subtituloRepository = subtituloRepository;
        }

        public bool ValidarSeArquivoExiste(string caminhoArquivo)
        {
            try
            {         
                bool arquivoExiste = System.IO.File.Exists(caminhoArquivo);
                if (!arquivoExiste)
                    throw new ArgumentException("Carregue um arquivo diferente");

               

                return true;
            }catch(Exception)
            {
                return false;
            }
        }

        public bool ValidarExtensaoArquivo(string caminhoArquivo)
        {
            try
            {
                string ext = System.IO.Path.GetExtension(caminhoArquivo);
                if (ext != null && ext.ToLower() != ".srt")
                    throw new ArgumentException("Extensão do arquivo incorreta");
                return true;
            }
            catch(Exception)
            {
                return false;
            }

        }

        public string CriarPastas(string nome, string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return Path.Combine(path, nome);
            }catch(Exception)
            {
                return "";
            }
        }

        public bool Validarsubtitulo(string arquivo, out List<Subtitulo> lsSubtitulo)
        {
            lsSubtitulo = new List<Subtitulo>();
            if (arquivo != null && arquivo.Length > 0)
            {
                string[] arLinhasFormatadas = System.Text.RegularExpressions.Regex.Replace(arquivo, "\r\n?", "\n").Split('\n');

                Subtitulo subtitulo = null;
                bool ultimaLinhaVazia = false;
                int passo = 0;

                try
                {
                    for (int i = 0; i < arLinhasFormatadas.Length; i++)
                    {
                        int seqOrdinaria;
                        bool linhaEmBranco = arLinhasFormatadas[i] == "";
                        bool linhaEhNumero = int.TryParse(arLinhasFormatadas[i], out seqOrdinaria);

                        if (linhaEhNumero && ultimaLinhaVazia)
                        {
                            passo = 0;
                            ultimaLinhaVazia = false;
                            lsSubtitulo.Add(subtitulo);
                            subtitulo = null;
                        }

                        switch (passo)
                        {
                            case 0: //Inicio de um subtitulo com seq numerica
                                if (!linhaEmBranco)
                                {
                                    subtitulo = new Subtitulo();
                                    subtitulo.numeroSequencial = seqOrdinaria;
                                    passo++;
                                }
                                break;

                            case 1:// Linha com inicio e fim do periodo da legenda
                                string[] arTempo = arLinhasFormatadas[i].Split("-->");
                                if (arTempo.Length == 2)
                                {
                                    TimeSpan tsIncial;
                                    TimeSpan tsFinal;

                                    bool tsInicialEhvalido = TimeSpan.TryParse(arTempo[0].Trim(), out tsIncial);
                                    bool tsFinalEhvalido = TimeSpan.TryParse(arTempo[1].Trim(), out tsFinal);

                                    if (tsInicialEhvalido && tsFinalEhvalido && tsFinal > tsIncial)
                                    {
                                        subtitulo.tempoInicial = tsIncial;
                                        subtitulo.tempoFinal = tsFinal;
                                        passo++;
                                        break;
                                    }
                                }
                                throw new ArgumentException("Erro ao parsear tempo das legendas");

                            case 2://Adciona legenda até aparecer uma linha em branco
                                if (!linhaEmBranco)
                                {
                                    subtitulo.linhasLegenda.Add(arLinhasFormatadas[i]);
                                    break;
                                }
                                ultimaLinhaVazia = true;
                                break;
                        }


                    }
                    if (subtitulo != null)
                        lsSubtitulo.Add(subtitulo);// Adciona a ultima legenda

                    var existeSequenciaRepetida = lsSubtitulo.GroupBy(f => f.numeroSequencial).Where(f => f.Count() > 1).ToList();

                    if (existeSequenciaRepetida.Count > 0)
                        throw new ArgumentException("Número sequencial repetido. Legenda inválida");
                    if(lsSubtitulo.Count == 1 && lsSubtitulo[0].linhasLegenda.Count == 0)
                        throw new ArgumentException("Subtitulo inválido");


                    lsSubtitulo.OrderBy(f => f.numeroSequencial).ToList();

                    return true;

                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        public byte[] CriarSubtituloAjustado(List<Subtitulo> lsSubtitulo, double incrementoSegundosIniciais, double incrementoSegundosFinais)
        {
            TimeSpan tmInicial = TimeSpan.FromSeconds(incrementoSegundosIniciais);
            TimeSpan tmFinal = TimeSpan.FromSeconds(incrementoSegundosFinais);


            lsSubtitulo.Select(subtitulo =>
            {
                subtitulo.tempoInicial = subtitulo.tempoInicial.Add(tmInicial);
                subtitulo.tempoFinal = subtitulo.tempoFinal.Add(tmFinal);
                return subtitulo;
            });

            StringBuilder sb = new StringBuilder();
            foreach (var subtitulo in lsSubtitulo)
            {
                sb.AppendLine(subtitulo.numeroSequencial.ToString());

                //hh:mm:ss,mss
                sb.AppendLine(subtitulo.tempoInicial.ToString(@"hh\:mm\:ss\,fff") + " --> " +
                              subtitulo.tempoFinal.ToString(@"hh\:mm\:ss\,fff"));

                subtitulo.linhasLegenda.ForEach(f => sb.AppendLine(f));
                sb.AppendLine();
            }

            byte[] arBytes = null;
            using (var ms = new MemoryStream())
            {
                TextWriter TW = new StreamWriter(ms);
                TW.Write(sb.ToString());
                TW.Flush();
                ms.Position = 0;
                arBytes = ms.ToArray();
            }
            return arBytes;
        }

        public void Dispose()
        {
            _subtituloRepository?.Dispose();
        }
    }
}
