using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public sealed class SettingsHelper
{
    public bool sound { get; set; }

    public bool fx { get; set; }

    public bool socialReported { get; set; }

    private List<ScoreModel> scores;
    private readonly string fileName;
    private static SettingsHelper helper = null;

    public static SettingsHelper SharedInstance()
    {
        if (helper == null)
        {
            helper = new SettingsHelper();
        }

        return helper;
    }

    private SettingsHelper()
    {
        fileName = Path.Combine(Application.persistentDataPath, "DuckyGhostSettingsEx.dat");
        sound = true;
        fx = true;
        socialReported = false;
        scores = new List<ScoreModel>();
        Load();
    }

    public int GetTotalScore()
    {
        int result = 0;

        foreach (ScoreModel child in scores)
        {
            result += child.score;
        }

        return result;
    }

    public void SetLevelScore(int levelNumber, int score)
    {
        ScoreModel model = scores.Find(m => m.levelNumber == levelNumber);

        if (model != null)
        {
            if (score > model.score)
            {
                model.score = score;
            }
        } else
        {
            model = new ScoreModel();
            model.levelNumber = levelNumber;
            model.score = score;
            scores.Add(model);
        }
    }

    public void Save()
    {
        try
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(string.Format(Constants.FORMAT_FIELD_VALUE, Constants.FIELD_NAME_SOUND, sound.ToString()));
                    writer.WriteLine(string.Format(Constants.FORMAT_FIELD_VALUE, Constants.FIELD_NAME_FX, fx.ToString()));
                    writer.WriteLine(string.Format(Constants.FORMAT_FIELD_VALUE, Constants.FIELD_NAME_SOCIAL_REPORTED, socialReported.ToString()));
                    writer.WriteLine(Constants.SECTION_NAME_LEVEL);

                    foreach (ScoreModel child in scores)
                    {
                        writer.WriteLine(string.Format(Constants.FORMAT_FIELD_VALUE, child.levelNumber, child.score));
                    }

                    writer.Flush();
                    writer.Close();
                }
                
                fs.Close();
            }
        } catch
        {
        }
    }

    private void Load()
    {
        try
        {
            if (File.Exists(fileName))
            {
                scores.Clear();

                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    string[] separator = new string[]{Constants.FIELD_VALUE_SEPARATOR};
                    bool section = false;
                    
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (Constants.SECTION_NAME_LEVEL.Equals(line))
                        {
                            section = true;
                        } else
                        {
                            string[] data = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            string fieldName = data[0];
                            string fieldValue = data[1];

                            if (section)
                            {
                                scores.Add(new ScoreModel(){levelNumber = int.Parse(fieldName), score = int.Parse(fieldValue)});
                            } else
                            {
                                if (Constants.FIELD_NAME_SOUND.Equals(fieldName))
                                {
                                    sound = bool.Parse(fieldValue);
                                } else if (Constants.FIELD_NAME_FX.Equals(fieldName))
                                {
                                    fx = bool.Parse(fieldValue);
                                } else if (Constants.FIELD_NAME_SOCIAL_REPORTED.Equals(fieldName))
                                {
                                    socialReported = bool.Parse(fieldValue);
                                }
                            }
                        }
                    }
                }
            }
        } catch
        {
        }
    }

    public int GetOpenedMaxLevelNumber()
    {
        int result = 0;
        
        foreach (ScoreModel child in scores)
        {
            result = Math.Max(result, child.levelNumber);
        }
        
        return result;
    }

    private sealed class ScoreModel
    {
        public int levelNumber { get; set; }

        public int score { get; set; }
    }
}