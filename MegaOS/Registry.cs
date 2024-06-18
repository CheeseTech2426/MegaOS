using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS {
    public class Registry {

        public readonly string registry = @"0:\MegaOS\sys\registry.mreg";

        

        public void ResetRegistry() {
         //   File.WriteAllLines(registry, CoreServices.registryDefault);
        }

        public void CheckRegistry() {
            CoreServices core = new CoreServices();
            if (!File.Exists(registry)) {
                core.Log($"Registry file not found at {registry}", LogType.Error);
                return;
            }
            int i = 0;
            try {
                Dictionary<string, Dictionary<string, string>> registryDict = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
                string[] lines = File.ReadAllLines(registry);
                string currentHeader = null;

                // Parse the registry file into a dictionary
                foreach (string line in lines) {
                    string trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]")) {
                        currentHeader = trimmedLine.Trim('[', ']');
                        if (!registryDict.ContainsKey(currentHeader)) {
                            registryDict[currentHeader] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        }
                    } else if (currentHeader != null && trimmedLine.Contains('=')) {
                        string[] keyValue = trimmedLine.Split(new[] { '=' }, 2);
                        if (keyValue.Length == 2) {
                            string key = keyValue[0].Trim();
                            string value = keyValue[1].Trim();
                            registryDict[currentHeader][key] = value;
                        }
                    }
                }

                // Check against the registryDefault array
                currentHeader = null;
                foreach (string line in CoreServices.defaultRegistry) {
                    string trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]")) {
                        currentHeader = trimmedLine.Trim('[', ']');
                        if (!registryDict.ContainsKey(currentHeader)) {
                            registryDict[currentHeader] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            core.Log($"Missing header added: {currentHeader}", LogType.Warning);
                            i++;
                        }
                    } else if (currentHeader != null && trimmedLine.Contains('=')) {
                        string[] keyValue = trimmedLine.Split(new[] { '=' }, 2);
                        if (keyValue.Length == 2) {
                            string key = keyValue[0].Trim();
                            string value = keyValue[1].Trim();
                            if (!registryDict[currentHeader].ContainsKey(key)) {
                                registryDict[currentHeader][key] = value;
                                core.Log($"Missing key added under header '{currentHeader}': {key}={value}", LogType.Warning);
                                i++;
                            }
                        }
                    }
                }

                // Write the updated registry back to the file
                using (StreamWriter writer = new StreamWriter(registry, false)) {
                    foreach (var header in registryDict) {
                        writer.WriteLine($"[{header.Key}]");
                        foreach (var keyValue in header.Value) {
                            writer.WriteLine($"{keyValue.Key}={keyValue.Value}");
                        }
                    }
                }
            } catch (Exception ex) {
                core.Log($"Error checking registry: {ex.Message}", LogType.Error);
                return;
            }
            core.Log($"Registry checked successfully and fixed {i} problem(s)!", LogType.OK);
        }

        public void CheckRegistry(string registryFile, string defaultRegistryFile) {
            CoreServices core = new CoreServices();
            int issuesFixed = 0;

            try {
                // Read registry file into a dictionary
                Dictionary<string, Dictionary<string, string>> registryDict = ReadRegistryFile(registryFile);

                // Read default registry file into a dictionary
                Dictionary<string, Dictionary<string, string>> defaultRegistryDict = ReadRegistryFile(defaultRegistryFile);

                // Check registry against default registry
                foreach (var defaultHeader in defaultRegistryDict) {
                    string headerName = defaultHeader.Key;
                    if (!registryDict.ContainsKey(headerName)) {
                        registryDict[headerName] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        core.Log($"Missing header added: [{headerName}]", LogType.Warning);
                        issuesFixed++;
                    }

                    foreach (var defaultKeyValue in defaultHeader.Value) {
                        string key = defaultKeyValue.Key;
                        if (!registryDict[headerName].ContainsKey(key)) {
                            string value = defaultKeyValue.Value; // Only use default value's key, not value
                            registryDict[headerName][key] = value;
                            core.Log($"Missing key added under header '[{headerName}]': {key}={value}", LogType.Warning);
                            issuesFixed++;
                        }
                    }
                }

                // Write the updated registry back to the file
                WriteRegistryFile(registryFile, registryDict);

                core.Log($"Registry checked successfully and fixed {issuesFixed} problem(s)!", LogType.OK);
            } catch (Exception ex) {
                core.Log($"Error checking registry: {ex.Message}", LogType.Error);
            }
        }

        private Dictionary<string, Dictionary<string, string>> ReadRegistryFile(string filePath) {
            Dictionary<string, Dictionary<string, string>> registryDict = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            if (!File.Exists(filePath)) {
                throw new FileNotFoundException($"Registry file not found at {filePath}");
            }

            string[] lines = File.ReadAllLines(filePath);
            string currentHeader = null;

            foreach (string line in lines) {
                string trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]")) {
                    currentHeader = trimmedLine.Trim('[', ']');
                    if (!registryDict.ContainsKey(currentHeader)) {
                        registryDict[currentHeader] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    }
                } else if (currentHeader != null && trimmedLine.Contains('=')) {
                    string[] keyValue = trimmedLine.Split(new[] { '=' }, 2);
                    if (keyValue.Length == 2) {
                        string key = keyValue[0].Trim();
                        string value = keyValue[1].Trim();
                        registryDict[currentHeader][key] = value;
                    }
                }
            }

            return registryDict;
        }

        private void WriteRegistryFile(string filePath, Dictionary<string, Dictionary<string, string>> registryDict) {
            using (StreamWriter writer = new StreamWriter(filePath, false)) {
                foreach (var header in registryDict) {
                    writer.WriteLine($"[{header.Key}]");
                    foreach (var keyValue in header.Value) {
                        writer.WriteLine($"{keyValue.Key}={keyValue.Value}");
                    }
                }
            }
        }


        public string GetValue(string header, string key) {
            if (!File.Exists(registry)) {
                CoreServices core = new CoreServices();
                core.Log($"Error: Registry file not found at {registry}", LogType.Panic);
                return null;
            }

            try {
                string[] lines = File.ReadAllLines(registry);
                bool inCorrectHeader = false;

                foreach (string line in lines) {
                    string trimmedLine = line.Trim();

                    // Check for header
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]")) {
                        string currentHeader = trimmedLine.Trim('[', ']');
                        inCorrectHeader = currentHeader.Equals(header, StringComparison.OrdinalIgnoreCase);
                    }
                    // Check for key=value pair
                    else if (inCorrectHeader && trimmedLine.Contains('=')) {
                        string[] keyValue = trimmedLine.Split(new[] { '=' }, 2);
                        if (keyValue.Length == 2) {
                            string currentKey = keyValue[0].Trim();
                            string currentValue = keyValue[1].Trim();

                            if (currentKey.Equals(key, StringComparison.OrdinalIgnoreCase)) {
                                return currentValue;
                            }
                        }
                    }
                }

                Console.WriteLine($"Warning: Key '{key}' not found under header '{header}'");
                return null;
            } catch (Exception ex) {
                Console.WriteLine($"Error reading registry: {ex.Message}");
                return null;
            }
        }

        public void SetValue(string header, string key, string newValue) {
            if (!File.Exists(registry)) {
                CoreServices core = new CoreServices();
                core.Log($"Error: Registry file not found at {registry}", LogType.Panic);
            }

            try {
                string[] lines = File.ReadAllLines(registry);
                List<string> newLines = new List<string>();
                bool inCorrectHeader = false;
                bool valueSet = false;

                foreach (string line in lines) {
                    string trimmedLine = line.Trim();

                    // Check for header
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]")) {
                        if (inCorrectHeader && !valueSet) {
                            // We reached a new header and didn't set the value yet, so add the new key=value before the new header
                            newLines.Add($"{key}={newValue}");
                            valueSet = true;
                        }
                        string currentHeader = trimmedLine.Trim('[', ']');
                        inCorrectHeader = currentHeader.Equals(header, StringComparison.OrdinalIgnoreCase);
                    }

                    if (inCorrectHeader && trimmedLine.Contains('=')) {
                        string[] keyValue = trimmedLine.Split(new[] { '=' }, 2);
                        if (keyValue.Length == 2) {
                            string currentKey = keyValue[0].Trim();

                            if (currentKey.Equals(key, StringComparison.OrdinalIgnoreCase)) {
                                // Update the value for the existing key
                                newLines.Add($"{key}={newValue}");
                                valueSet = true;
                                continue;
                            }
                        }
                    }

                    // Add the original line to the new lines list
                    newLines.Add(line);
                }

                if (!valueSet) {
                    // If we didn't set the value, it means either the header or key was not found. Add them to the end of the file.
                    if (!inCorrectHeader) {
                        newLines.Add($"[{header}]");
                    }
                    newLines.Add($"{key}={newValue}");
                }

                // Write the updated lines back to the registry file
                File.WriteAllLines(registry, newLines);
            } catch (Exception ex) {
                Console.WriteLine($"Error writing to registry: {ex.Message}");
            }
        }

        public void AddKey(string header, string key, string value) {
            if (!File.Exists(registry)) {
                CoreServices core = new CoreServices();
                core.Log($"Error: Registry file not found at {registry}", LogType.Panic);
            }

            try {
                string[] lines = File.ReadAllLines(registry);
                List<string> newLines = new List<string>();
                bool inCorrectHeader = false;
                bool keyAdded = false;

                foreach (string line in lines) {
                    string trimmedLine = line.Trim();

                    // Check for header
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]")) {
                        if (inCorrectHeader && !keyAdded) {
                            // Add the new key=value before the new header
                            newLines.Add($"{key}={value}");
                            keyAdded = true;
                        }
                        string currentHeader = trimmedLine.Trim('[', ']');
                        inCorrectHeader = currentHeader.Equals(header, StringComparison.OrdinalIgnoreCase);
                    }

                    // If in correct header and key is not already present
                    if (inCorrectHeader && trimmedLine.Contains('=')) {
                        string[] keyValue = trimmedLine.Split(new[] { '=' }, 2);
                        if (keyValue.Length == 2 && keyValue[0].Trim().Equals(key, StringComparison.OrdinalIgnoreCase)) {
                            Console.WriteLine($"Warning: Key '{key}' already exists under header '{header}'. Use SetValue to update it.");
                            return;
                        }
                    }

                    newLines.Add(line);
                }

                if (!keyAdded) {
                    if (!inCorrectHeader) {
                        newLines.Add($"[{header}]");
                    }
                    newLines.Add($"{key}={value}");
                }

                File.WriteAllLines(registry, newLines);
            } catch (Exception ex) {
                Console.WriteLine($"Error writing to registry: {ex.Message}");
            }
        }
        public void RemoveKey(string header, string key) {
            if (!File.Exists(registry)) {
                CoreServices core = new CoreServices();
                core.Log($"Error: Registry file not found at {registry}", LogType.Panic);
            }

            try {
                string[] lines = File.ReadAllLines(registry);
                List<string> newLines = new List<string>();
                bool inCorrectHeader = false;

                foreach (string line in lines) {
                    string trimmedLine = line.Trim();

                    // Check for header
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]")) {
                        string currentHeader = trimmedLine.Trim('[', ']');
                        inCorrectHeader = currentHeader.Equals(header, StringComparison.OrdinalIgnoreCase);
                    }

                    // If in correct header and key matches, skip this line
                    if (inCorrectHeader && trimmedLine.Contains('=')) {
                        string[] keyValue = trimmedLine.Split(new[] { '=' }, 2);
                        if (keyValue.Length == 2 && keyValue[0].Trim().Equals(key, StringComparison.OrdinalIgnoreCase)) {
                            continue;
                        }
                    }

                    newLines.Add(line);
                }

                File.WriteAllLines(registry, newLines);
            } catch (Exception ex) {
                Console.WriteLine($"Error writing to registry: {ex.Message}");
            }
        }

        public void AddHeader(string header) {
            if (!File.Exists(registry)) {
                CoreServices core = new CoreServices();
                core.Log($"Error: Registry file not found at {registry}", LogType.Panic);
            }

            try {
                string[] lines = File.ReadAllLines(registry);
                foreach (string line in lines) {
                    if (line.Trim().Equals($"[{header}]", StringComparison.OrdinalIgnoreCase)) {
                        Console.WriteLine($"Warning: Header '{header}' already exists.");
                        return;
                    }
                }

                using (StreamWriter sw = File.AppendText(registry)) {
                    sw.WriteLine($"[{header}]");
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error writing to registry: {ex.Message}");
            }
        }

        public void RemoveHeader(string header) {
            if (!File.Exists(registry)) {
                CoreServices core = new CoreServices();
                core.Log($"Error: Registry file not found at {registry}", LogType.Panic);
            }

            try {
                string[] lines = File.ReadAllLines(registry);
                List<string> newLines = new List<string>();
                bool inCorrectHeader = false;

                foreach (string line in lines) {
                    string trimmedLine = line.Trim();

                    // Check for header
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]")) {
                        string currentHeader = trimmedLine.Trim('[', ']');
                        inCorrectHeader = currentHeader.Equals(header, StringComparison.OrdinalIgnoreCase);

                        // If this is the header to remove, skip it
                        if (inCorrectHeader) {
                            continue;
                        }
                    }

                    // If in correct header, skip the lines until next header
                    if (inCorrectHeader) {
                        if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]")) {
                            inCorrectHeader = false;
                        } else {
                            continue;
                        }
                    }

                    newLines.Add(line);
                }

                File.WriteAllLines(registry, newLines);
            } catch (Exception ex) {
                Console.WriteLine($"Error writing to registry: {ex.Message}");
            }
        }

    }
}
