string line = "[Margaret Ann Lass (5 Apr 1947 - Present)]-[Ronald Merrigan (29 Mar 1945 - 6 Aug 1972)]: 25 Jun 1965";
string[] separator = new string[] {"[", "]-[", "]: "};
string[] parts = line.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
foreach (string part in parts)
{
    Console.WriteLine(part);
}

