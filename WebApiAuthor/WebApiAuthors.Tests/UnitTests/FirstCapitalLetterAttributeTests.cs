using System.ComponentModel.DataAnnotations;
using WebApiAuthor.Validations;

namespace WebApiAuthors.Tests.UnitTests;

[TestClass]
public class FirstCapitalLetterAttributeTests
{
    [TestMethod]
    public void FirstLowercaseLetter_ReturnError()
    {
        //Preparation
        var firstCapitalLetter = new FirstCapitalLetterAttribute();
        var value = "roman";
        var valContext = new ValidationContext(new {Name = value});
        
        //Execute
        var result = firstCapitalLetter.GetValidationResult(value, valContext);

        //Validations
        Assert.AreEqual("The first letter must be uppercase", result.ErrorMessage);
    }
    
    [TestMethod]
    public void NullValue_NotReturnError()
    {
        //Preparation
        var firstCapitalLetter = new FirstCapitalLetterAttribute();
        string value = null;
        var valContext = new ValidationContext(new {Name = value});
        
        //Execute
        var result = firstCapitalLetter.GetValidationResult(value, valContext);

        //Validations
        Assert.IsNull(result);
    }
}