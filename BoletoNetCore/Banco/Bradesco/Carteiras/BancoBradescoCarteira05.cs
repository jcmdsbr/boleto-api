﻿using System;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore.Banco.Bradesco.Carteiras
{
    [CarteiraCodigo("05")]
    public class BancoBradescoCarteira05 : ICarteira<BancoBradesco>
    {
        private BancoBradescoCarteira05()
        {
        }

        internal static Lazy<ICarteira<BancoBradesco>> Instance { get; } =
            new Lazy<ICarteira<BancoBradesco>>(() => new BancoBradescoCarteira05());

        public void FormataNossoNumero(Boleto.Boleto boleto)
        {
            // Nosso número não pode ter mais de 11 dígitos

            if (IsNullOrWhiteSpace(boleto.NossoNumero) || boleto.NossoNumero == "00000000000")
            {
                // Banco irá gerar Nosso Número
                boleto.NossoNumero = new string('0', 11);
                boleto.NossoNumeroDV = "0";
                boleto.NossoNumeroFormatado = "000/00000000000-0";
            }
            else
            {
                // Nosso Número informado pela empresa
                if (boleto.NossoNumero.Length > 11)
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 11 dígitos.");
                boleto.NossoNumero = boleto.NossoNumero.PadLeft(11, '0');
                boleto.NossoNumeroDV = (boleto.Carteira + boleto.NossoNumero).CalcularDVBradesco();
                boleto.NossoNumeroFormatado =
                    $"{boleto.Carteira.PadLeft(3, '0')}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
            }
        }

        public string FormataCodigoBarraCampoLivre(Boleto.Boleto boleto)
        {
            var contaBancaria = boleto.Banco.Beneficiario.ContaBancaria;
            return $"{contaBancaria.Agencia}{boleto.Carteira}{boleto.NossoNumero}{contaBancaria.Conta}{"0"}";
        }
    }
}