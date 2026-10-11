using FuzzySearchProject.Entities;
using FuzzySearchProject.Service;

ItemService itemService = new ItemService();

List<ItemEntity> GenerateDummyItems = itemService.GenerateDummyItems();

string searchTerm = "Safty";

// Search items using FuzzySearch within all string fields of ItemEntity
List<ItemEntity> searchItems1 = FuzzySearch.Search(
    GenerateDummyItems,
    searchTerm);

// Search items using FuzzySearch within specific field of ItemEntity
List<ItemEntity> searchItems2 = FuzzySearch.Search(
    GenerateDummyItems,
    searchTerm,
    x=>x.ItemName);

// Search items using FuzzySearch within multiple specific fields of ItemEntity
List<ItemEntity> searchItems3 = FuzzySearch.Search(
    GenerateDummyItems,
    searchTerm,
    x => x.ItemName,
    x => x.Description);

Console.ReadLine();


