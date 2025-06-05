using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class CEnemyTemplateList
    {
        private List<CEnemyTemplate> enemies;

        public CEnemyTemplateList() => enemies = new List<CEnemyTemplate>();

        public void AddEnemy(string name, string iconName, int baseLife,
                        double lifeModifier, int baseGold,
                        double goldModifier, double spawnChance)
        {
            enemies.Add(new CEnemyTemplate(name, iconName, baseLife,
                                         lifeModifier, baseGold,
                                         goldModifier, spawnChance));
        }

        public CEnemyTemplate GetEnemyByName(string name) =>
        enemies.Find(e => e.Name() == name);

        public CEnemyTemplate GetEnemyByIndex(int id) =>
            (id >= 0 && id < enemies.Count) ? enemies[id] : null;

        public void DeleteEnemyByName(string name) =>
            enemies.RemoveAll(e => e.Name() == name);

        public void DeleteEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count) enemies.RemoveAt(id);
        }

        public List<string> GetEnemyNames()
        {
            List<string> names = new List<string>();
            foreach (var enemy in enemies) names.Add(enemy.Name());
            return names;
        }

        public void SaveToJson(string path)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            List<Dictionary<string, object>> serializableList = new();

            foreach (var enemy in enemies)
            {
                serializableList.Add(new Dictionary<string, object>
                {
                    ["name"] = enemy.Name(),
                    ["iconName"] = enemy.IconName(),
                    ["baseline"] = enemy.Baselife(),
                    ["lifeModifier"] = enemy.LifeModifier(),
                    ["baseGold"] = enemy.BaseGold(),
                    ["goldModifier"] = enemy.GoldModifier(),
                    ["spawnChance"] = enemy.SpawnChance()
                });
            }

            File.WriteAllText(path, JsonSerializer.Serialize(serializableList, options));
        }

        public void LoadFromJson(string path)
        {
            string json = File.ReadAllText(path);
            using JsonDocument doc = JsonDocument.Parse(json);

            enemies.Clear();
            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                enemies.Add(new CEnemyTemplate(
                    element.GetProperty("name").GetString(),
                    element.GetProperty("iconName").GetString(),
                    element.GetProperty("baseline").GetInt32(),
                    element.GetProperty("lifeModifier").GetDouble(),
                    element.GetProperty("baseGold").GetInt32(),
                    element.GetProperty("goldModifier").GetDouble(),
                    element.GetProperty("spawnChance").GetDouble()
                ));
            }
        }
    }
}
