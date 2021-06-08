using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MMiners.Bussiness.Models;

namespace MMiners.Bussiness.Interfaces
{
    public interface ISubtituloService : IDisposable
    {
        //Task<bool> Adicionar(int ms);
        bool ValidarSeArquivoExiste(string caminhoArquivo);
        bool ValidarExtensaoArquivo(string caminhoArquivo);
        string CriarPastas(string nome, string path);
        bool Validarsubtitulo(string arquivo, out List<Subtitulo> ls);
        byte[] CriarSubtituloAjustado(List<Subtitulo> lsSubtitulo, double incrementoSegundosIniciais, double incrementoSegundosFinais);
    }
}
