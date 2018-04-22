<Query Kind="Program" />

void Main()
{
	string line = "1,1b0279.054,7268.867,954.601,DTM";
	var cell = line.Split(',');

    MalformedCell(cell);
	CheckLineLength(cell);
	
	
}

// Define other methods and classes here
public static bool MalformedCell(string[] cell)
{
	try
	{
		Double.Parse(cell[1]);
		Double.Parse(cell[2]);
		Double.Parse(cell[3]);
	}
	catch (Exception ex)
	{
		return true;
	}

	return false;
}

public static bool CheckLineLength(string[] cell)
{
	if (cell.Length != 5)
		return false;
	return true;
}