Affector
===
This library generates method for updating a entity set from a string.
This uses [Sprache](https://github.com/sprache/Sprache) for parsing the string.

Usage
===
Define class as below.
Mark Entity as `[Affectable]` , public key properties as `[Key(index)]` and public value properties as `[Value(names)]`.
```csharp
class EntitySet
{
    public Entity[] E1 { get; set; }
}

[Affectable]
class Entity
{
    [Key(0)]
    public string Key1 { get; set; }
    [Key(1)]
    public string Key2 { get; set; }
    [Value("", "V1")]
    public double Value1 { get; set; }
    [Value("", "V2")]
    public double Value2 { get; set; }
}
```
Then, call
```
var entitySet = new EntitySet();
// initialize entitySet...
Func<EntitySet, EntitySet> f = Affector.Generate<EntitySet>(script);
var updatedEntitySet = f(entitySet);
```
The scripts that can be described in this example are as below.
```
E1(value operation for Value1 and Value2)
E1(condition for A, value operation for Value1 and Value2)
E1(condition for A, condition for B, value operation for Value1 and Value2)

E1.V1(value operation for Value1)
E1.V1(condition for Key1, value operation for Value1)
E1.V1(condition for Key1, condition for Key2, value operation for Value1)

E1.V2(value operation for Value2)
E1.V2(condition for Key1, value operation for Value2)
E1.V2(condition for Key1, condition for Key2, value operation for Value2)
```
It is possible to combine the above functions with comma.
```
E1.V1(value operation for Value1), E1.V2(condition for A, value operation for Value2)
```
Script example.
```
// both value1 and value2 update to value+=100
E1(+0.01)
// assign 0.01 to value1 if Key1=="A"
E1.V1(="A", =0.01)
// value1 -=0.01 if Key1 is not A && Key2 is X or Y
E1.V1(!="A", ="X" || ="Y", -0.01)
// value1 += 0.01 if Key1=="A"
// value2 -= 0.01 if Key1=="A"
E1.V1(="A", +0.01),E1.V2(="A", -0.01)
```
Author
===
Yamagoke is a software developer in Japan.

License
===
This library is under the MIT License.
