global using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.CharacterService
{
  public class CharacterService : ICharacterService
  {
    private static List<Character> characters = new List<Character>{
        new Character(),
        new Character {Id = 1, Name = "erdem"},
    };
    private readonly IMapper _mapper;
    public readonly DataContext _context;

    public CharacterService(IMapper mapper, DataContext context)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
      var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
      var character = _mapper.Map<Character>(newCharacter);
      character.Id = characters.Max(c => c.Id) + 1;
      characters.Add(character);
      serviceResponse.Data = characters.Select(a => _mapper.Map<GetCharacterDto>(a)).ToList();
      return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
    {
      var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
      try
      {
        var character = characters.FirstOrDefault(a => a.Id == id);
        if (character is null)
        {
          throw new Exception($"Character with Id '{id}' not Found");
        }
        characters.Remove(character);

        serviceResponse.Data = characters.Select(a => _mapper.Map<GetCharacterDto>(a)).ToList();
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
      var dbCharacters = await _context.Characters.ToArrayAsync();
      serviceResponse.Data = dbCharacters.Select(a => _mapper.Map<GetCharacterDto>(a)).ToList();
      return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();
      var dbCharacter = await _context.Characters.FirstOrDefaultAsync(a => a.Id == id);
      serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
      return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();
      try
      {
        var character = characters.FirstOrDefault(a => a.Id == updatedCharacter.Id);
        if (character is null)
        {
          throw new Exception($"Character with Id '{updatedCharacter.Id}' not Found");
        }
        _mapper.Map(updatedCharacter, character);

        character.Name = updatedCharacter.Name;
        character.HitPoints = updatedCharacter.HitPoints;
        character.Strength = updatedCharacter.Strength;
        character.Intelligence = updatedCharacter.Intelligence;
        character.Class = updatedCharacter.Class;
        serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
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