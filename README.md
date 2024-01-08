# ZooLink
An ASP.NET API used for assigning animals to zoo enclosures

# Endpoints
```/api/Animals``` - Get/Create/Delete Animals <br>
```/api/Enclosures``` - Get/Create/Delete operations for Enclosures <br>
```/api/Dev``` - Used To populate tables with data <br>
Before running any ```Animal``` or ```Enclosure``` endpoints populate ```ZooAssets```, available ```AnimalTypes``` and ```AnimalPreferredAssets``` tables


# Rules
- Vegetarian animals can be placed together in the same enclosure
- A Herbivore cannot be in the same cage as a Carnivore and vice versa
- Meat-eating animals are preferably not grouped together, but if necessary only two different species of meat-eating animals can be grouped together
- Animals of the same species are preferred to be assigned to the same enclosure
- Enclosures with a Species favorable objects are of higher priority

# Models
In order to add Enclosure or Animals to the zoo an array of object is necessary
## Enclosure
```json
{
  "enclosures": [
    {
      "name": "string",
      "size": "Small",
      "location": "Inside",
      "objects": [
        "string"
      ]
    }
  ]
}
```
### Location
Enclosures can be located ```Inside``` or ```Outside```
### Size
Available Sizes:
| Size   | Integer |
|--------|---------|
| Small  | 5       |
| Medium | 10      |
| Large  | 15      |
| Huge   | 20      |

### Model
On a successful POST request a list of added enclosures is returned

## Animal
```json
{
  "animals": [
    {
      "species": "string",
      "food": "string",
      "amount": 0
    }
  ]
}
```
### Food
Food is meant to represent the diet of the animal. An animal can be a ```Herbivore``` or a ```Carnivore```

### Model
On a successful POST request a list of added animals is returned. Animals which could not be added are not returned in the list


