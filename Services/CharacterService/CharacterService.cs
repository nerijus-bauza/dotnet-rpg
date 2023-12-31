using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace dotnet_rpg.Services.CharacterService
{
  public class CharacterService : ICharacterService
  {
    
    private readonly IMapper mapper;
    private readonly DataContext context;

    public CharacterService(IMapper mapper, DataContext context)
    {
      this.context = context;
      this.mapper = mapper;
    }
    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
      var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

      var character = mapper.Map<Character>(newCharacter);

      context.Characters.Add(character);
      await context.SaveChangesAsync();

      serviceResponse.Data = await context.Characters
        .Select(c => mapper.Map<GetCharacterDto>(c))
        .ToListAsync();
      
      serviceResponse.Message = $"New character with id '{character.Id}' successfully added.";

      return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
    {
      var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

      try
      {
        var character = await context.Characters.FirstOrDefaultAsync(c => c.Id == id);
        if (character is null)
        {
          throw new Exception($"Character with Id '{id}' not found.");
        }

        context.Characters.Remove(character);
        await context.SaveChangesAsync();

        serviceResponse.Data = await context.Characters
          .Select(c => mapper.Map<GetCharacterDto>(c))
          .ToListAsync();
        serviceResponse.Message = $"Character with Id '{id}' successfully deleted.";
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Message = ex.Message;
      }

      return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
      var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
      var dbCharacters = await context.Characters.ToListAsync();
      serviceResponse.Data = dbCharacters
        .Select(c => mapper.Map<GetCharacterDto>(c))
        .ToList();

      return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();
      var dbCharacter = await context.Characters.FirstOrDefaultAsync(character => character.Id == id);
      serviceResponse.Data = mapper.Map<GetCharacterDto>(dbCharacter);

      return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();

      try
      {
        var character = await context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
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

        await context.SaveChangesAsync();
        serviceResponse.Data = mapper.Map<GetCharacterDto>(character);
        serviceResponse.Message = $"Character with Id '{updatedCharacter.Id}' successfully updated.";
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