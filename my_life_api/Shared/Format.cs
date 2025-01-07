namespace my_life_api.Shared;

public static class Format {
    public static string GetWordWithUpperCaseInitial(string word) { 
        return char.ToUpper(word[0]) + word[1..];
    }

    public static byte GetByteValueOfBool(bool value) {
        return (byte)(value == true ? 1 : 0);
    }
}
