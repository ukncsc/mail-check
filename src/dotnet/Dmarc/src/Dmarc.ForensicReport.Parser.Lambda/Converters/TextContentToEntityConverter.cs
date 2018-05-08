using System.Collections.Generic;
using System.Linq;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Utils;
using Dmarc.ForensicReport.Parser.Lambda.Domain;

namespace Dmarc.ForensicReport.Parser.Lambda.Converters
{
    public interface ITextContentToEntityConverter
    {
        ForensicTextEntity Convert(TextContent textContent, int order);
    }

    public class TextContentToEntityConverter : ITextContentToEntityConverter
    {
        private readonly IForensicUriToEntityConverter _forensicUriToEntityConverter;

        public TextContentToEntityConverter(IForensicUriToEntityConverter forensicUriToEntityConverter)
        {
            _forensicUriToEntityConverter = forensicUriToEntityConverter;
        }

        public ForensicTextEntity Convert(TextContent textContent, int order)
        {
            List<HashEntity> hashes = textContent.Hashes.Select(_ => new HashEntity((EntityHashType)(int)_.HashType, _.Hash)).ToList();
            List<ForensicTextContentUriEntity> uris = textContent.Urls.Select(_ => new ForensicTextContentUriEntity(_forensicUriToEntityConverter.Convert(_))).ToList();
            ForensicTextContentEntity forensicTextContentEntity = new ForensicTextContentEntity(textContent.RawContent, hashes, uris);

            return new ForensicTextEntity(forensicTextContentEntity, new ContentTypeEntity(textContent.ContentType), order, textContent.Depth);
        }
    }
}
