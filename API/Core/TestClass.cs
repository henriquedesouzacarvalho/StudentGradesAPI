using System;

namespace StudentGradesAPI.Core;

public class TestClass
{
    public void BadMethod()
    {
        var unused = "test"; // Variável não utilizada
        try
        {
            // Código que pode falhar
            var result = 1 / 0;
            Console.WriteLine(result);
        }
        catch (Exception)
        {
            // Sem tratamento
        }
    }
}
