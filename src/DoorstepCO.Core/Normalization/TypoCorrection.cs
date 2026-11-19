namespace DoorstepCO.Core.Normalization;

/// <summary>
/// Detects common typos: a word that is at most one edit away from a keyword, where an edit is inserting,
/// deleting or replacing one letter, or swapping two adjacent letters ("carrerra", "calel").
/// </summary>
internal static class TypoCorrection
{
    public static bool IsWithinOneEdit(string word, string keyword)
    {
        if (Math.Abs(word.Length - keyword.Length) > 1)
        {
            return false;
        }

        var distance = new int[word.Length + 1, keyword.Length + 1];
        for (var i = 0; i <= word.Length; i++)
        {
            distance[i, 0] = i;
        }

        for (var j = 0; j <= keyword.Length; j++)
        {
            distance[0, j] = j;
        }

        for (var i = 1; i <= word.Length; i++)
        {
            for (var j = 1; j <= keyword.Length; j++)
            {
                var cost = word[i - 1] == keyword[j - 1] ? 0 : 1;
                distance[i, j] = Math.Min(
                    Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                    distance[i - 1, j - 1] + cost);

                if (i > 1 && j > 1 && word[i - 1] == keyword[j - 2] && word[i - 2] == keyword[j - 1])
                {
                    distance[i, j] = Math.Min(distance[i, j], distance[i - 2, j - 2] + 1);
                }
            }
        }

        return distance[word.Length, keyword.Length] <= 1;
    }
}
