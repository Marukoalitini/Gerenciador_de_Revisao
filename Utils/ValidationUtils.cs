using System.Text.RegularExpressions;

namespace Motos.Utils;

public static class ValidationUtils
{
    public static string SomenteNumeros(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return valor;
        return Regex.Replace(valor, @"[^\d]", "");
    }

    public static bool IsCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;

        cpf = Regex.Replace(cpf, @"[^\d]", "");

        if (cpf.Length != 11) return false;

        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        string digito = resto.ToString();
        tempCpf = tempCpf + digito;
        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito = digito + resto.ToString();

        return cpf.EndsWith(digito);
    }

    public static bool IsCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj)) return false;

        cnpj = Regex.Replace(cnpj, @"[^\d]", "");

        if (cnpj.Length != 14) return false;

        if (new string(cnpj[0], 14) == cnpj) return false;

        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCnpj = cnpj.Substring(0, 12);
        int soma = 0;

        for (int i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        int resto = (soma % 11);
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        string digito = resto.ToString();
        tempCnpj = tempCnpj + digito;
        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = (soma % 11);
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito = digito + resto.ToString();

        return cnpj.EndsWith(digito);
    }

    public static bool IsTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone)) return false;
        
        string apenasNumeros = Regex.Replace(telefone, @"[^\d]", "");
        
        return apenasNumeros.Length >= 10 && apenasNumeros.Length <= 13;
    }

    public static bool IsSenhaSegura(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha)) return false;

        // Mínimo 8 caracteres, pelo menos uma maiúscula, uma minúscula, um número e um caractere especial
        bool temTamanhoMinimo = senha.Length >= 8;
        bool temMaiuscula = Regex.IsMatch(senha, @"[A-Z]");
        bool temMinuscula = Regex.IsMatch(senha, @"[a-z]");
        bool temNumero = Regex.IsMatch(senha, @"[0-9]");
        bool temEspecial = Regex.IsMatch(senha, @"[\W_]");

        return temTamanhoMinimo && temMaiuscula && temMinuscula && temNumero && temEspecial;
    }
}