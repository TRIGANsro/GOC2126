class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public static Person[] GetPeople(int count)
    {
        Random random = new Random();
        var people = new Person[count];
        for (int i = 0; i < count; i++)
        {
            people[i] = new Person { Name = "Person" + i, Age = random.Next(20, 40) };
        }
        return people;
    }
}