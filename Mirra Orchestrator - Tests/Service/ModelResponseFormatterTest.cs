using Mirra_Orchestrator.Service;

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
    }
}
