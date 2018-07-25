using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace csFinalProj1
{
    class Program
    {
        static List<string> all_genres = new List<string>(File.ReadAllLines(@"C:\Users\Joel Gahr\Desktop\c# projects\csFinalProj1\csFinalProj1\existingGenres.txt"));

        static void Main(string[] args)
        {
            bool doReset = false;
            string[] reset_number = File.ReadAllLines("TempMov.txt");
            if (reset_number.Length > 7 || reset_number.Length == 0 || reset_number[0] == "")
            {
                File.WriteAllText("TempMov.txt", "");
                doReset = true;
            }
            




            // Make a new list of movie objects
            var movie_list = new List<Movie>();

            // Make a new list of movie genres
            List<string> genreList = new List<string>();
            //int genre_count = 0;

            // This will help us with indexing and also setting the max number of times that loops should loop
            int specific_movie = 0;

            // Read in our file of movies line by line into the variable reader, which is of type StreamReader
            using (StreamReader reader = new StreamReader(@"C:\Users\Joel Gahr\Desktop\movieInfo.txt"))
            {

                string line;
                
                // go line by line through [reader], figuring out what to do with each word it contains
                while ((line = reader.ReadLine()) != null)
                {
                    
                    string[] row = line.Split('\t');


                    // add a new object to the list
                    movie_list.Add(new Movie());

                    // assign an i.d. to our Id variable
                    movie_list[specific_movie].Id = row[0];

                    // assign a value to our Title variable
                    movie_list[specific_movie].Title = row[2];

                    // assign an int value to our Year variable
                    movie_list[specific_movie].Year = int.Parse(row[5]);


                    // assign a value/values to our Genre variable
                    string[] genre = row[8].Split(',');
                    for (int i = 0; i < genre.Length; i++)
                    {
                        movie_list[specific_movie].Genre[i] = genre[i];

                        // Make a list of all the existing genres, which will be used to write a file which helps us choose genre parameters
                        //if (!(genreList.Contains(genre[i])))
                        //{
                        //    ++genre_count;
                        //    genreList.Add(genre[i]);
                        //    Console.WriteLine("Adding new genre...");
                        //}
                    
                        
                    }

                    // assign a value to our rating variable
                    movie_list[specific_movie].Rating = double.Parse(row[9]);


                    // increment through our list of movies, so that each object is a separate object with the appropriate attributes
                    ++specific_movie;
                }
            }

            // Add everything in the genre list to a file named existingGenres.txt
            //for (int g_index= 0; g_index < genre_count; g_index++)
            //{
            //    File.AppendAllText(@"C:\Users\Joel Gahr\Desktop\c# projects\csFinalProj1\csFinalProj1\existingGenres.txt", genreList[g_index] + '\n');
            //}


            var checkList = new List<bool> { false, false, false, true };
            Movie new_movie = new Movie();
            Movie picked_movie = new Movie();
            var perm_movie = new List<string>(File.ReadAllLines("PermMov.txt"));


            do
            {
                if (doReset == false)
                {
                    new_movie = GenerateParameters();
                }
                else
                {
                    new_movie.Genre[0] = getGenre(all_genres);

                    List<int> all_years = new List<int>();
                    for (int index = 0, year = 1880; index < 137; index++, year++)
                    {
                        all_years.Add(year);
                    }
                    new_movie.Year = getYear(all_years);

                    List<double> good_ratings = new List<double>();
                    for (double i = 7.0; i < 10.0; i += 0.1)
                    {
                        good_ratings.Add(i);
                    }
                    new_movie.Rating = Math.Round(getRating(good_ratings), 1);
                }   
                Console.WriteLine(new_movie.Rating);
                Console.WriteLine(new_movie.Year);
                Console.WriteLine(new_movie.Genre[0]);
                Console.WriteLine("\n\n");


           

                // Problem: It is choosing the same selections on a loop it almost seems like. Abandon, Aria, Duets.
                //List<Movie> shuffled = RandomizeStrings(movie_list);

                for (int i = 0; i < movie_list.Count; i++)
                {
                    for (int check_i = 0; check_i < 4; check_i++)
                    {
                        checkList[check_i] = false;

                        if (check_i == 3)
                        {
                            checkList[check_i] = true;
                        }
                    }

                    
                    for (int perm_i = 0; perm_i < perm_movie.Count; perm_i++)
                    {
                        if (perm_movie[perm_i] == movie_list[i].Id)
                        {
                            checkList[3] = false;
                        }
                    }

                    for (int genre_i = 0; genre_i < 3; genre_i++)
                    {
                        if (movie_list[i].Genre[genre_i] == new_movie.Genre[0])
                        {
                            checkList[0] = true;
                            break;
                        }
                    }
                    if ((Math.Abs(movie_list[i].Rating - new_movie.Rating) < 1) && checkList[0] == true)
                    {
                        checkList[1] = true;
                    }
                    
                    if (movie_list[i].Year == new_movie.Year && checkList[1] == true)
                    {
                        checkList[2] = true;
                        
                        picked_movie = movie_list[i];
                        string temp_file_path = "TempMov.txt";
                        copyTempToFile(picked_movie, temp_file_path);


                        var new_perm_movie = new List<string>();
                        new_perm_movie.Add(movie_list[i].Id);
                        File.AppendAllLines("PermMov.txt", new_perm_movie);

                        break;
                    }
                    

                    

                }
            } while (checkList[0] == false || checkList[1] == false || checkList[2] == false || checkList[3] == false);

            Console.WriteLine(picked_movie.Title);
            Console.WriteLine(picked_movie.Year);
            Console.ReadLine();






        }

     

        public static void copyTempToFile(Movie movie, string file_path)
        {
            var new_temp_line = new List<string>();
            new_temp_line.Add(movie.Rating.ToString() + '\t' + movie.Year.ToString() + '\t' + movie.Genre[0] + '\t' + movie.Genre[1] + '\t' + movie.Genre[2]);

            File.AppendAllLines(file_path, new_temp_line);
        }


        // YAY!
        static Random r = new Random();



        //Make a method that randomly generates parameters to be chosen for the next movie.
        public static Movie GenerateParameters()
        {




            // look up recently used genres, dates, and ratings

            // Check to see which genres are available


            // keeps track of which genres will be excluded
            List<string> excludedGenre = new List<string>();
            List<string> includedGenre = new List<string>();
 

            foreach (string temp_line in File.ReadLines("TempMov.txt"))
            {
                string[] temp_row = temp_line.Split('\t');
                if (temp_row.Length > 2)
                    excludedGenre.Add(temp_row[2]);
                if (temp_row.Length > 3)
                    excludedGenre.Add(temp_row[3]);
                if (temp_row.Length > 4)
                    excludedGenre.Add(temp_row[4]);
            }
            for (int excluded_i = 0; excluded_i < excludedGenre.Count; excluded_i++)
            {
               
                
                for (int genre_i = 0; genre_i < all_genres.Count; genre_i++)
                {
                    if (excludedGenre[excluded_i] == all_genres[genre_i])
                    {
                        all_genres[genre_i] = "";
                        break;
                    }
                }
                
                    
                
            }
            for (int genre_i = 0; genre_i < all_genres.Count; genre_i++)
            {
                if (all_genres[genre_i] != "")
                    includedGenre.Add(all_genres[genre_i]);
                
            }
            //foreach (var item in includedGenre)
            //{
            //    Console.WriteLine(item);
            //}




            // Check to see which years are available
            var excludedEras = new List<int>();
            var includedEras = new List<bool>() { true, true, true, true, true, true, true, true };
            var includedYears = new List<int>();
          
            

            foreach (string temp_line in File.ReadLines("TempMov.txt"))
            {
                string[] temp_row = temp_line.Split('\t');
                excludedEras.Add(int.Parse(temp_row[1])); 
            }
            for (int excluded_i = 0; excluded_i < excludedEras.Count; excluded_i++)
            {

                if (excludedEras[excluded_i] < 1925)
                {
                    includedEras[0] = false;
                }
                else if (excludedEras[excluded_i] < 1941 && excludedEras[excluded_i] >= 1925)
                {
                    includedEras[1] = false;
                }
                else if (excludedEras[excluded_i] < 1960 && excludedEras[excluded_i] >= 1941)
                {
                    includedEras[2] = false;
                }
                else if (excludedEras[excluded_i] < 1970 && excludedEras[excluded_i] >= 1960)
                {
                    includedEras[3] = false;
                }
                else if (excludedEras[excluded_i] < 1980 && excludedEras[excluded_i] >= 1970)
                {
                    includedEras[4] = false;
                }
                else if (excludedEras[excluded_i] < 1990 && excludedEras[excluded_i] >= 1980)
                {
                    includedEras[5] = false;
                }
                else if (excludedEras[excluded_i] < 2000 && excludedEras[excluded_i] >= 1990)
                {
                    includedEras[6] = false;
                }
                else
                    includedEras[7] = false;

            }
            if (includedEras[0])
            {
                for (int i = 1880; i < 1925; i++)
                {
                    includedYears.Add(i);
                }
            }
            if (includedEras[1])
            {
                for (int i = 1925; i < 1941; i++)
                {
                    includedYears.Add(i);
                }
            }
            if (includedEras[2])
            {
                for (int i = 1941; i < 1960; i++)
                {
                    includedYears.Add(i);
                }
            }
            if (includedEras[3])
            {
                for (int i = 1960; i < 1970; i++)
                {
                    includedYears.Add(i);
                }
            }
            if (includedEras[4])
            {
                for (int i = 1970; i < 1980; i++)
                {
                    includedYears.Add(i);
                }
            }
            if (includedEras[5])
            {
                for (int i = 1980; i < 1990; i++)
                {
                    includedYears.Add(i);
                }
            }
            if (includedEras[6])
            {
                for (int i = 1990; i < 2000; i++)
                {
                    includedYears.Add(i);
                }
            }
            if (includedEras[7])
            {
                for (int i = 2000; i < 2020; i++)
                {
                    includedYears.Add(i);
                }
            }
            //foreach (var item in includedYears)
            //{
            //    Console.WriteLine(item);
            //}



            // Check to see which Ratings are available


            var tempRatings = new List<double>();
            var includedRatings = new List<double>();

            foreach (string temp_line in File.ReadLines("TempMov.txt"))
            {
                string[] temp_row = temp_line.Split('\t');
                tempRatings.Add(double.Parse(temp_row[0]));
            }

          
            if (tempRatings.Average() > 6.38)
            {
              for ( double low_ratings = 0.0;  low_ratings < 6.38; low_ratings += 0.1)
                {
                    includedRatings.Add(low_ratings);
                }
            }
            else
            {
                for (double low_ratings = 6.38; low_ratings <=10.0; low_ratings += 0.1)
                {
                    includedRatings.Add(low_ratings);
                }
            }


            // Create a Movie object from the remaining availabilities.
            // Has blank title and ID obviously
            Movie nextMovie = new Movie();
            nextMovie.Genre[0] = getGenre(includedGenre);
            nextMovie.Year = getYear(includedYears);
            nextMovie.Rating = Math.Round(getRating(includedRatings),1);

            return nextMovie;
            



            // Compare this object with all Movie objects with titles/ID's.
            // If none found, go back to [Create a Movie object from the remaining availabilities.
            // if one or more are found, put all these titled/ID'd objects into a list.
            // use length of newly created list to initialize a rng's parameters.
            // Roll the dice
            // Match the number rolled to the List item with the identical index
            // Check to make sure this movie isn't already in the permanent file
            // if it is, reroll
            // Add this object to a permanent file.
            // Add this object's to a temporary file that is overwritten every fifth time that a movie is selected.
            // Write the object's title and year of release to the console.
        }
        public static string getGenre(List<string> genres)
        {
            string genre = "";

            
            int roll = r.Next(0, genres.Count);

            for (int i = 0; i < genres.Count; i++)
            {
                if (roll == i)
                {
                    genre = genres[i];
                    break;
                }
            }

            return genre;
        }
        public static int getYear(List<int> years)
        {
            int year = 0;
            int roll = r.Next(0, years.Count);

            for (int i = 0; i < years.Count; i++)
            {
                if (roll == i)
                {
                    year = years[i];
                    break;
                }
            }


            return year;
        }
        public static double getRating(List<double> ratings)
        {
            double rating = 0.0;
            int roll = r.Next(0, ratings.Count);

            for (int i = 0; i < ratings.Count; i++)
            {
                if (roll == i)
                {
                    rating = ratings[i];
                    break;
                }
            }


            return rating;
        }
    }

    class Movie
    {
        public Movie()
        {
            Id = "";
            Title = "";
            Rating = 0.0;
            Year = 0;

            for (int i = 0; i < 3; i++)
            {
                Genre[i] = "";
            }

            //Country = "";
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public double Rating { get; set; }
        public int Year { get; set; }
        public string[] Genre = new string[3];
        //public string Country { get; set; }


    }
   
}
