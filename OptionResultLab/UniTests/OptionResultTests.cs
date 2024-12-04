using Monads;

namespace UniTests;

public class OptionResultTests
{
    [Test]
    public void Deve_conter_valor_valido()
    {
        OptionResult<string> optString = "Hello, World!";
        Assert.That(optString.HasValue, Is.True);
        Assert.That(optString.HasError, Is.False);
    }

    [Test]
    public void Nao_deve_conter_valor()
    {
        string teste = null!;
        OptionResult<string> optString = teste;
        Assert.That(optString.HasValue, Is.False);
        Assert.That(optString.HasError, Is.False);
    }

    [Test]
    public void Deve_conter_erro()
    {
        OptionResult<string> optString = new Error("Ocorreu um erro.");
        Assert.That(optString.HasValue, Is.False);
        Assert.That(optString.HasError, Is.True);
    }

    [Test]
    public void Match_deve_retornar_valor_correto()
    {
        OptionResult<string> optString = "Hello, World!";
        var result = optString.Match(
            some: value => value,
            none: () => "Nenhum valor",
            error: error => error.Message
        );
        Assert.That(result, Is.EqualTo("Hello, World!"));
    }

    [Test]
    public void Match_deve_retornar_ausencia_de_valor()
    {
        string teste = null!;
        OptionResult<string> optString = teste;
        var result = optString.Match(
            some: value => value,
            none: () => "Nenhum valor",
            error: error => error.Message
        );
        Assert.That(result, Is.EqualTo("Nenhum valor"));
    }

    [Test]
    public void Match_deve_retornar_ocorrencia_de_erro()
    {
        OptionResult<string> optString = new Error("Erro");
        var result = optString.Match(
            some: value => value,
            none: () => "Nenhum valor",
            error: error => error.Message
        );
        Assert.That(result, Is.EqualTo("Erro"));
    }

    [Test]
    public void Then_deve_retornar_valor_correto()
    {
        OptionResult<string> optString = "Hello, World!";

        OptionResult<decimal> decRes = 12345M;
        
        var res = optString.Then(s =>
                           {
                               Assert.That(s, Is.EqualTo("Hello, World!"));
                               return Unit.Value;
                           })
                           .Then(u =>
                           {
                               Assert.That(u, Is.EqualTo(Unit.Value));
                               return 13;
                           })
                           .Then(i =>
                           {
                               Assert.That(i, Is.EqualTo(13));
                               return decRes;
                           })
                           .Match(
                               some: v => v,
                               none: () => 0,
                               error: e => -1
                           );

        Assert.That(res, Is.EqualTo(12345M));
    }
}
