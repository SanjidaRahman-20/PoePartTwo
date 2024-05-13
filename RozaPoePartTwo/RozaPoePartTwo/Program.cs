using System;
using System.Collections.Generic;
using System.Linq;

// Delegate for notification when a recipe exceeds 300 calories
public delegate void RecipeNotification(string recipeName);

// Class representing a recipe
public class Recipe
{
    // Properties
    public string Name { get; set; } // Name of the recipe
    public Dictionary<string, (double quantity, string unit, double calories, string foodGroup)> Ingredients { get; set; } // Ingredients of the recipe
    public List<string> Steps { get; set; } // Steps to prepare the recipe

    // Constructor
    public Recipe()
    {
        Ingredients = new Dictionary<string, (double, string, double, string)>(); // Initialize ingredients dictionary
        Steps = new List<string>(); // Initialize steps list
    }

    // Method to add an ingredient to the recipe
    public void AddIngredient(string name, double quantity, string unit, double calories, string foodGroup)
    {
        Ingredients.Add(name, (quantity, unit, calories, foodGroup)); // Add the ingredient to the dictionary
    }

    // Method to add a step to the recipe
    public void AddStep(string step)
    {
        Steps.Add(step); // Add the step to the list
    }

    // Method to calculate total calories of the recipe
    public double CalculateTotalCalories()
    {
        double totalCalories = 0;
        foreach (var ingredient in Ingredients)
        {
            totalCalories += ingredient.Value.calories * ingredient.Value.quantity;
        }
        return totalCalories;
    }
}

// Class for managing recipes
public class RecipeManager
{
    private Dictionary<string, Recipe> recipes; // Dictionary to store recipes by name
    public event RecipeNotification OnRecipeExceedsCalories; // Event for notifying when a recipe exceeds 300 calories

    // Constructor
    public RecipeManager()
    {
        recipes = new Dictionary<string, Recipe>(); // Initialize the dictionary
    }

    // Method to add a recipe to the manager
    public void AddRecipe(string name, Recipe recipe)
    {
        recipes.Add(name, recipe); // Add the recipe to the dictionary

        // Calculate total calories
        double totalCalories = recipe.CalculateTotalCalories();

        // Display total calories
        Console.WriteLine($"Total Calories for {name}: {totalCalories}");

        // Check if total calories exceed 300 and raise event
        if (totalCalories > 300)
        {
            OnRecipeExceedsCalories?.Invoke(name);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Warning: Total calories exceed 300!");
            Console.ResetColor();
        }
    }

    // Method to remove a recipe from the manager
    public void RemoveRecipe(string name)
    {
        recipes.Remove(name); // Remove the recipe from the dictionary
    }

    // Method to display all recipes in alphabetical order
    public List<string> DisplayRecipesAlphabetically()
    {
        Console.WriteLine("Recipes (Alphabetical Order):");
        List<string> recipeNames = new List<string>();
        foreach (var recipe in recipes.OrderBy(x => x.Key))
        {
            Console.WriteLine($"- {recipe.Key}");
            recipeNames.Add(recipe.Key);
        }
        return recipeNames;
    }

    // Method to get a recipe by name
    public Recipe GetRecipe(string name)
    {
        return recipes.ContainsKey(name) ? recipes[name] : null; // Return the recipe if found, otherwise return null
    }

    // Method to scale the quantities of ingredients in a recipe
    public void ScaleRecipe(string name, double factor)
    {
        if (recipes.ContainsKey(name)) // Check if the recipe exists
        {
            Recipe recipe = recipes[name]; // Get the recipe
            foreach (var ingredient in recipe.Ingredients) // Iterate over each ingredient
            {
                // Scale the quantity of the ingredient by the factor
                var updatedIngredient = (quantity: ingredient.Value.quantity * factor, unit: ingredient.Value.unit, calories: ingredient.Value.calories, foodGroup: ingredient.Value.foodGroup);
                recipe.Ingredients[ingredient.Key] = updatedIngredient; // Update the ingredient in the recipe
            }
        }
    }
}

class Program
{
    // Main method
    static void Main(string[] args)
    {
        RecipeManager recipeManager = new RecipeManager(); // Create an instance of RecipeManager

        // Subscribe to the recipe notification event
        recipeManager.OnRecipeExceedsCalories += RecipeExceedsCaloriesHandler;

        while (true) // Main menu loop
        {
            Console.Clear(); // Clear the console
            Console.WriteLine("Choose an option:"); // Display menu options
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("1. Add Recipe");
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("2. Remove Recipe");
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("3. Display Recipes");
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("4. Scale Recipe");
            Console.ResetColor();

            string choice = Console.ReadLine(); // Get user choice

            switch (choice) // Perform action based on choice
            {
                case "1": // Add Recipe
                    AddRecipe(recipeManager); // Call method to add recipe
                    break;

                case "2": // Remove Recipe
                    Console.Write("Enter the name of the recipe to remove: ");
                    string recipeToRemove = Console.ReadLine();
                    recipeManager.RemoveRecipe(recipeToRemove); // Call method to remove recipe
                    Console.WriteLine($"Recipe '{recipeToRemove}' removed.");
                    GoBack(); // Display message and wait for user input
                    break;

                case "3": // Display Recipes
                    DisplayRecipe(recipeManager); // Call method to display recipes
                    break;

                case "4": // Scale Recipe
                    ScaleRecipe(recipeManager); // Call method to scale recipe
                    break;

                case "5": // Exit
                    Console.WriteLine("Exiting..."); // Display exit message
                    Environment.Exit(0); // Exit the program
                    break;

                default: // Invalid choice
                    Console.WriteLine("Invalid choice. Please try again."); // Display error message
                    GoBack(); // Display message and wait for user input
                    break;
            }
        }
    }

    // Method to add a recipe
    static void AddRecipe(RecipeManager recipeManager)
    {
        Console.Write("Enter recipe name: ");
        string recipeName = Console.ReadLine(); // Get recipe name
        Recipe recipe = new Recipe(); // Create a new Recipe object
        recipe.Name = recipeName; // Set recipe name

        Console.Write("Enter number of ingredients: ");
        if (!int.TryParse(Console.ReadLine(), out int numIngredients) || numIngredients <= 0) // Get number of ingredients
        {
            Console.WriteLine("Invalid input for number of ingredients.");
            GoBack();
            return;
        }
        for (int i = 0; i < numIngredients; i++) // Loop to input ingredients
        {
            Console.Write($"Enter ingredient {i + 1} name: ");
            string ingredientName = Console.ReadLine(); // Get ingredient name
            Console.Write($"Enter quantity for {ingredientName}: ");
            if (!double.TryParse(Console.ReadLine(), out double quantity) || quantity <= 0) // Get ingredient quantity
            {
                Console.WriteLine("Invalid input for quantity.");
                GoBack();
                return;
            }
            Console.Write($"Enter unit for {ingredientName}: ");
            string unit = Console.ReadLine(); // Get ingredient unit

            // Get additional ingredient information
            Console.Write($"Enter calories for {ingredientName}: ");
            if (!double.TryParse(Console.ReadLine(), out double calories) || calories <= 0)
            {
                Console.WriteLine("Invalid input for calories.");
                GoBack();
                return;
            }

            Console.Write($"Enter food group for {ingredientName}: ");
            string foodGroup = Console.ReadLine(); // Get food group

            recipe.AddIngredient(ingredientName, quantity, unit, calories, foodGroup); // Add ingredient to recipe
        }

        Console.Write("Enter number of steps: ");
        if (!int.TryParse(Console.ReadLine(), out int numSteps) || numSteps <= 0) // Get number of steps
        {
            Console.WriteLine("Invalid input for number of steps.");
            GoBack();
            return;
        }
        for (int i = 0; i < numSteps; i++) // Loop to input steps
        {
            Console.Write($"Enter step {i + 1}: ");
            string step = Console.ReadLine(); // Get step
            recipe.AddStep(step); // Add step to recipe
        }

        recipeManager.AddRecipe(recipeName, recipe); // Add recipe to manager
        GoBack(); // Display message and wait for user input
    }

    // Method to display a recipe
    static void DisplayRecipe(RecipeManager recipeManager)
    {
        List<string> recipeNames = recipeManager.DisplayRecipesAlphabetically();
        Console.Write("Enter the name of the recipe to display: ");
        string recipeToDisplay = Console.ReadLine();
        if (recipeNames.Contains(recipeToDisplay))
        {
            Recipe displayedRecipe = recipeManager.GetRecipe(recipeToDisplay);
            // Display the chosen recipe
            if (displayedRecipe != null)
            {
                Console.WriteLine($"Recipe: {displayedRecipe.Name}");
                Console.WriteLine("Ingredients:");
                foreach (var ingredient in displayedRecipe.Ingredients)
                {
                    Console.WriteLine($"- {ingredient.Key}: {ingredient.Value.quantity} {ingredient.Value.unit}");
                }
                Console.WriteLine("Steps:");
                for (int i = 0; i < displayedRecipe.Steps.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {displayedRecipe.Steps[i]}");
                }
            }
            else
            {
                Console.WriteLine($"Recipe '{recipeToDisplay}' not found.");
            }
        }
        else
        {
            Console.WriteLine($"Recipe '{recipeToDisplay}' not found.");
        }
        GoBack(); // Display message and wait for user input
    }

    // Method to scale a recipe
    static void ScaleRecipe(RecipeManager recipeManager)
    {
        List<string> recipeNamesToScale = recipeManager.DisplayRecipesAlphabetically();
        Console.Write("Enter the name of the recipe to scale: ");
        string recipeToScale = Console.ReadLine();
        if (recipeNamesToScale.Contains(recipeToScale))
        {
            Console.Write("Enter scale factor: ");
            if (double.TryParse(Console.ReadLine(), out double factor)) // Get scale factor
            {
                recipeManager.ScaleRecipe(recipeToScale, factor); // Scale the recipe
                Console.WriteLine($"Recipe '{recipeToScale}' scaled by a factor of {factor}.");
            }
            else
            {
                Console.WriteLine("Invalid input for scale factor.");
            }
        }
        else
        {
            Console.WriteLine($"Recipe '{recipeToScale}' not found.");
        }
        GoBack(); // Display message and wait for user input
    }

    // Method to handle notification when a recipe exceeds 300 calories
    static void RecipeExceedsCaloriesHandler(string recipeName)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Warning: Recipe '{recipeName}' exceeds 300 calories!");
        Console.ResetColor();
    }

    // Method to display a message and wait for user input to go back to the main menu
    static void GoBack()
    {
        Console.WriteLine("\nPress Enter to go back to the main menu...");
        Console.ReadLine();
    }
}
