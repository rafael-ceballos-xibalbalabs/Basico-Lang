namespace Basico
{
    public class SourceCodeLoader
    {
        private string _sourceCodePath;
        public SourceCodeLoader(string sourceCodePath) 
        {
            _sourceCodePath = sourceCodePath;
        }
        public string ReadSourceCode()
        {
            if (string.IsNullOrEmpty(_sourceCodePath))
            {
                throw new System.ArgumentException("El path del código fuente no puede ser nulo o vacío.");
            }
            if (!System.IO.File.Exists(_sourceCodePath))
            {
                throw new System.IO.FileNotFoundException($"El archivo '{_sourceCodePath}' no existe.");
            }
            return System.IO.File.ReadAllText(_sourceCodePath);
        }
    }
}
