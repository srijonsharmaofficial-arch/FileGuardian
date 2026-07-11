using System.Collections.Generic;
using System.Linq;
using FileGuardian.Interfaces;

namespace FileGuardian.Services
{
    /// <summary>
    /// Groups files by content hash to find duplicates.
    /// Depends on IHashGenerator instead of a specific hash algorithm,
    /// so we can swap HashService for a different one later without
    /// changing this class at all.
    /// </summary>
    public class DuplicateFinderService
    {
        private readonly IHashGenerator _hashGenerator;

        public DuplicateFinderService(IHashGenerator hashGenerator)
        {
            _hashGenerator = hashGenerator;
        }

        public List<List<string>> FindDuplicateGroups(List<string> filePaths)
        {
            Dictionary<string, List<string>> hashGroups = new Dictionary<string, List<string>>();

            foreach (string path in filePaths)
            {
                try
                {
                    string hash = _hashGenerator.ComputeHash(path);

                    if (!hashGroups.ContainsKey(hash))
                        hashGroups[hash] = new List<string>();

                    hashGroups[hash].Add(path);
                }
                catch
                {
                    // Skip files that can't be read (locked, permissions, etc.)
                }
            }

            return hashGroups.Values.Where(group => group.Count > 1).ToList();
        }
    }
}