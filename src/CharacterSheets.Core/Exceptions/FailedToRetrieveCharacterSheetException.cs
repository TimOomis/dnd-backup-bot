using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterSheets.Core.Exceptions;

public class FailedToRetrieveCharacterSheetException(Exception innerException) : Exception("Failed to retrieve the character sheet.", innerException)
{
}
