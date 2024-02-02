using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace SpaceScribble
{
    class CreditsManager
	{
        private static CreditsManager instance;

        private const string USERDATA_FILE = "user.txt";

        /// <summary>
        /// Defines the default player credits.
        /// </summary>
        private long totalCredits = 0;

        private CreditsManager()
		{
            this.loadUserData();
        }

        #region Methods

        public static CreditsManager GetInstance()
        {
            if (instance == null)
            {
                instance = new CreditsManager();
            }

            return instance;
        }

        public void IncreaseTotalCredits(long value)
        {
            this.totalCredits += value;
        }

        public void DecreaseTotalCredits(long value)
        {
            this.totalCredits -= value;
        }

        public void Save()
        {
            this.saveUserData();
        }

        private void saveUserData()
        {

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(USERDATA_FILE, FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.WriteLine(this.totalCredits);

                        sw.Flush();
                        sw.Close();
                    }
                }
            }
        }

        private void loadUserData()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(USERDATA_FILE);

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(USERDATA_FILE, FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            this.totalCredits = Int64.Parse(sr.ReadLine());
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            sw.WriteLine(this.totalCredits);

                            // ... ? 
                        }
                    }
                }
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.totalCredits = Int64.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(totalCredits);
        }

        #endregion

        #region Properties

        public long TotalCredits
        {
            get
            {
                return this.totalCredits;
            }
        }

        #endregion
    }
}

