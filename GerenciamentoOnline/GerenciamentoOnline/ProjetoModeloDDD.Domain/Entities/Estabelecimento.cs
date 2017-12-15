using Castle.ActiveRecord;
using kardapio.Suprimentos;
using Newtonsoft.Json;
using ProjetoModelo.Entities;
using ProjetoModeloDDD.Domain.Auxiliar;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoModeloDDD.Domain.Entities
{
    [ActiveRecord(Cache = CacheEnum.ReadWrite)]
    [System.ComponentModel.DefaultPropertyAttribute("Fantasia")]
    [Serializable]
    public class Estabelecimento : ClasseModelo
    {
        [PrimaryKey(Generator = PrimaryKeyType.Native)]
        public override int? Id { get; set; }

        [Property(Length = 80)]
        public string RazaoSocial { get; set; }

        [Property(Length = 60)]
        public string Fantasia { get; set; }


        // CNAE - Classificação Nacional de Atividades Econômicas
        [Property(Length = 14)]
        public string CNAE { get; set; }

        [Property(Length = 14)]
        public string CNPJ { get; set; }

        [Property(Length = 15)]
        public string Telefone { get; set; }

        [Property(Length = 120)]
        public string Logradouro { get; set; }

        [Property(Length = 10)]
        public string Numero { get; set; }

        [Property(Length = 80)]
        public string Complemento { get; set; }

        [UIHint("Search")]
        [Property(Length = 80)]
        public string CEP { get; set; }

        [BelongsTo(Fetch = FetchEnum.Select)]
        [UIHint("DropDown")]
        [JsonConverter(typeof(ClasseModeloJsonConvert))]
        public Cidade Cidade { get; set; }

        [BelongsTo(Fetch = FetchEnum.Select)]
        [UIHint("DropDown")]
        [JsonConverter(typeof(ClasseModeloJsonConvert))]
        public Bairro Bairro { get; set; }

        [Property(Length = 20)]
        public String InscricaoEstadual { get; set; }

        [Property(Length = 20)]
        public String InscricaoEstadualST { get; set; }

        [Property(Length = 20)]
        public String InscricaoMunicipal { get; set; }

        [Property(Length = 80)]
        public String ResponsavelNome { get; set; }

        [Property(Length = 15)]
        public String ResponsavelTelefone { get; set; }

        [Property(Length = 60)]
        public String Email { get; set; }

        [Property]
        [JsonConverter(typeof(NullDateJsonConvert))]
        public DateTime? DataInicioAtividades { get; set; }

        [Property]
        [JsonConverter(typeof(NullDateJsonConvert))]
        public DateTime? DataCadastro { get; set; }

        [Property]
        public int? CodNovoEstabelecimento { get; set; }

        public string NomeUsuarioTemp { get; set; }

        public string SenhaTemp { get; set; }

        public string SenhaTempDescriptografada { get; set; }

        [Property(Length = 50)]
        public string LoginCadastro { get; set; }


        public override void Inicializa(Estabelecimento estab)
        {
            base.Inicializa(estab);
            this.DataCadastro = DateTime.Now;
        }


    }
}