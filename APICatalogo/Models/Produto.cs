using APICatalogo.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APICatalogo.Models
{
    [Table("Produtos")]
    public class Produto : IValidatableObject
    {
        [Key]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage ="O nome é obrigatório")]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "o nome deve ter no mínimo 5 e no máximo 80 caracteres")]
        [PrimeiraLetraMaiuscula]
        public string? Nome { get; set; }

        [Required(ErrorMessage ="A descrição é obrigatória")]
        [StringLength(300)]
        public string? Descricao { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage ="O preço deve estar entre 1 e 10000")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Preco { get; set; }

        [Required]
        [StringLength(300, MinimumLength=10, ErrorMessage = "O campo ImagemUrl deve ser uma string com comprimento mínimo de 10 e máximo de 300.")]
        public string? ImagemUrl { get; set; }

        public float Estoque { get; set; }

        public DateTime DataCadastro { get; set; }

        public int CategoriaId { get; set; }
        
        [JsonIgnore]
        public Categoria? Categoria { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(this.Estoque < 0)
            {
                yield return new ValidationResult("O estoque deve ser negativo",
                    new[]
                    {nameof(this.Estoque)}
                    );
            }
        }
    }
}