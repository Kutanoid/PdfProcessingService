using Microsoft.Extensions.Options;
using PdfProcessingService.API.Authentication;
using PdfProcessingService.API.Exceptions;

namespace PdfProcessingService.API.Configurations
{
    public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
    {
        private const string JWT_CONFIGURATION_SECTION = "JwtConfigurationOptions";

        private readonly IConfiguration _configuration;

        public JwtOptionsSetup(IConfiguration configuration) { 
            _configuration = configuration;
        }

        public void Configure(JwtOptions options)
        {
            var section = _configuration.GetSection(JWT_CONFIGURATION_SECTION);
            if(section == null) {
                throw new CouldntFindSectionException(JWT_CONFIGURATION_SECTION);
            }
            section.Bind(options);
        }
    }
}
