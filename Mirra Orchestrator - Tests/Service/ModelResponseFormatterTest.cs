using Mirra_Orchestrator.Service;
using Mirra_Orchestrator___Tests.Examples;

namespace Mirra_Orchestrator___Tests.Service
{
    [TestClass]
    public class ModelResponseFormatterTest
    {
        ModelResponseFormatter modelResponseFormatter = new ModelResponseFormatter();

        [TestMethod]
        public void removeSpecialCharactersFromImageCaptionsSuccess()
        {

            var validImageCaption = "[IMG: DESCRIÇÃO DA IMAGEM &&& *LEGENDA DA IMAGEM*]";
            var result = modelResponseFormatter.removeSpecialCharactersFromImageCaptions(validImageCaption);
            Assert.AreEqual("[IMG: DESCRIÇÃO DA IMAGEM &&& LEGENDA DA IMAGEM]", result);

        }


        [TestMethod]
        public void GetWordpressBlogPostFromModelResponseSuccess()
        {

            var modelResponse = ModelResponseExample.Text;
            var result = modelResponseFormatter.GetWordpressBlogPostFromModelResponse(modelResponse);
            Assert.AreEqual(ModelResponseExample.Title, result.title);
            Assert.AreEqual(ModelResponseExample.FormattedText, result.content);

        }
    }
}
