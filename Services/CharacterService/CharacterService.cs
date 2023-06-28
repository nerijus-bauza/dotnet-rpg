using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace dotnet_rpg.Services.CharacterService
{
  public class CharacterService : ICharacterService
  {
    private static List<Character> characters = new List<Character> {
        new Character(),
        new Character { Id = 1, Name = "Sam" }
    };
    private readonly IMapper mapper;

    public CharacterService(IMapper mapper)
    {
      this.mapper = mapper;
    }
    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
      var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

      var character = mapper.Map<Character>(newCharacter);
      character.Id = characters.Max(c => c.Id) + 1;

      characters.Add(character);
      serviceResponse.Data = characters
        .Select(c => mapper.Map<GetCharacterDto>(c))
        .ToList();

      return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
      var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
      serviceResponse.Data = characters
        .Select(c => mapper.Map<GetCharacterDto>(c))
        .ToList();

      return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();
      var character = characters.FirstOrDefault(character => character.Id == id);
      serviceResponse.Data = mapper.Map<GetCharacterDto>(character);

      return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();

      try
      {
        var character = characters.FirstOrDefault(c => c.Id == updatedCharacter.Id);
        if (character is null)
        {
          throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");
        }

        // update character with mapper
        mapper.Map(updatedCharacter, character);
        // below code is done with mapper (the line above)
        /*
        character.Name = updatedCharacter.Name;
        character.HitPoints = updatedCharacter.HitPoints;
        character.Strength = updatedCharacter.Strength;
        character.Defence = updatedCharacter.Defence;
        character.Intelligence = updatedCharacter.Intelligence;
        character.Class = updatedCharacter.Class;
        */
        serviceResponse.Data = mapper.Map<GetCharacterDto>(character);
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Message = ex.Message;
      }

      return serviceResponse;
    }
  }
}