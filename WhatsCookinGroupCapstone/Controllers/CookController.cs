﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WhatsCookinGroupCapstone.Contracts;
using WhatsCookinGroupCapstone.Models;

namespace WhatsCookinGroupCapstone.Controllers
{
    public class CookController : Controller
    {
        private IRepositoryWrapper _repo;

        public CookController(IRepositoryWrapper repo)
        {
            _repo = repo;

        }

        // GET: CookController
        // Default view: will show a grid of multiple recipes and cooks you are following
        public ActionResult Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var selectedCook = _repo.Cook.FindByCondition(r => r.IdentityUserId == userId).SingleOrDefault();
            
            if (selectedCook == null)
            {
                return RedirectToAction("Create");

            }
            else
            {
                List<Recipe> recipeList = FindMatchingRecipes(FindRecipeTagsMatchingCookTags(FindCookTags(selectedCook)));
                List<Recipe> finalRecipeList = RandomizeRecipes(recipeList);

                return View(finalRecipeList);
                return View(selectedCook);
            }

        }

        // GET: CookController/Details/5
        public ActionResult Details(int id)
        {
            var selectedCook = _repo.Cook.FindByCondition(r => r.CookId == 1).SingleOrDefault();
            return View(selectedCook);
        }

        // GET: CookController/Create
        public ActionResult Create()
        {
            Cook cook = new Cook();
            {
                cook.AllTags = GetTags();
            }

                    
            return View(cook);
        }

        // POST: CookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cook cook)
        {
 
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                cook.IdentityUserId = userId;
                _repo.Cook.Create(cook);
                _repo.Save();

                var selectedCook = _repo.Cook.FindByCondition(c => c.IdentityUserId == userId).SingleOrDefault();
                var selectedCookId = selectedCook.CookId;

                foreach (string tag in cook.SelectedTags)
                {
                    var selectedTag = _repo.Tags.FindByCondition(r => r.Name == tag).SingleOrDefault();

                    CookTag cookTag = new CookTag();
                    cookTag.CookId = selectedCookId;
                    cookTag.TagsId = selectedTag.TagsId;
                    _repo.CookTag.Create(cookTag);
                    _repo.Save();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cook);
        }
        

        // GET: CookController/Edit/5
        public ActionResult Edit(int id)
        {
            var selectedCook = _repo.Cook.FindByCondition(r => r.CookId == id).SingleOrDefault();
            return View(selectedCook);
        }

        // POST: CookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private IList<SelectListItem> GetTags()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Vegan", Value = "Vegan" },
                new SelectListItem { Text = "Paleo", Value = "Paleo" },
                new SelectListItem { Text = "Pescatarian", Value = "Pescatarian" },
                new SelectListItem { Text = "Nut-Free", Value = "Nut-Free" },
                new SelectListItem { Text = "Dairy", Value = "Dairy" }
            };

        }

        //I want to find all of the tags for my cook **
        // then, I want to find all recpies tagged with the same tags related to my cook**
        // i.e., cook only wants vegan recipes ("TagId = 1"), we then query the RecipeTags table and find all recipes with a
        // TagId == 1. I want to have a list of RecipeIds from this query**
        // then, I want to query my recipes table for all of the recipes that match list of RecipeIds and add them to a list**
        // Then, I want to pass six of those recipes to a view and display them as a grid that the cook can see
        // Ideally, I want to randomly select the six recipes
        private List<int> FindCookTags(Cook cook)
        {
            var selectedCook = _repo.Cook.FindByCondition(c => c.CookId == cook.CookId).SingleOrDefault();
            var cookTags = _repo.CookTag.FindByCondition(c => c.CookId == selectedCook.CookId);
            List<int> recipeTags = new List<int>();
            foreach (CookTag cookTag in cookTags)
            {
                recipeTags.Add(cookTag.TagsId);
            }
            return recipeTags;
        }
        private List<int> FindRecipeTagsMatchingCookTags(List<int> recipeTags)
        {
            List<int> recipeIds = new List<int>();
            foreach (int tagId in recipeTags)
            {
                var selectedRecipe = _repo.RecipeTags.FindByCondition(c => c.TagsId == tagId).FirstOrDefault();
                recipeIds.Add(selectedRecipe.RecipeId);
            }
            return recipeIds;
        }
        private List<Recipe> FindMatchingRecipes(List<int> recipeIds)
        {
            List<Recipe> recipeList = new List<Recipe>();
            foreach (int recipeId in recipeIds)
            {
                var selectedRecipe = _repo.Recipe.FindByCondition(c => c.RecipeId == recipeId).SingleOrDefault();
                recipeList.Add(selectedRecipe);
            }
            return recipeList;
        }
        private List<int> GetSixRandomNumbers(int recipeCount)
        {
            //Need to remember to make a check so that it doesn't create the same random number twice
            List<int> sixRandomNumbers = new List<int>();
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                sixRandomNumbers.Add(random.Next(recipeCount));
            }
            return sixRandomNumbers;
        }
        private List<Recipe> RandomizeRecipes(List<Recipe> recipeList)
        {
            // recipeList generated from FindMatchingRecipes
            // set int recipeCount parameter for GetSixRandomNumbers = to recipeList.Count-1
            int recipeCount = 0;
            
            if (recipeList.Count < 6)
            {
                var listOfAllRecipes = _repo.Recipe.FindAll().ToList();
                foreach(Recipe recipe in listOfAllRecipes)
                {
                    recipeCount++;
                }
               
            }
            else
            {
                recipeCount = recipeList.Count();
            }
            List<int> randomNumbers = GetSixRandomNumbers(recipeCount);

            List<Recipe> finalRecipeList = new List<Recipe>();
            if (recipeList.Count > 6)
            {
                foreach (int randomNumber in randomNumbers)
                {
                    var recipe = recipeList[randomNumber];
                    finalRecipeList.Add(recipe);
                }
            }
            else
            {
                foreach (int randomNumber in randomNumbers)
                {
                    var recipe = _repo.Recipe.FindByCondition(r => r.RecipeId == randomNumber).SingleOrDefault();
                    finalRecipeList.Add(recipe);
                }
            }
            return finalRecipeList;
        }
        //public ActionResult DefaultView(Cook cook)
        //{
        //    List<Recipe> recipeList = FindMatchingRecipes(FindRecipeTagsMatchingCookTags(FindCookTags(cook)));
        //    List<Recipe> finalRecipeList = RandomizeRecipes(recipeList);
        //    return View(finalRecipeList);
        //}
        private void RandomizeRecipes(List<int> sixNumbers, Cook cook)
        {
            var selectedCook = _repo.Cook.FindByCondition(c => c.CookId == cook.CookId).SingleOrDefault();
            var cookTags = _repo.CookTag.FindByCondition(c => c.CookId == selectedCook.CookId);
        }
        private List<int> GetSixRandomNumbers(RecipeTags recipeTags)
        {
            List<int> sixNumbers = new List<int>();
            Random random = new Random();
            var recipeListCount = _repo.RecipeTags.FindAll();
            for (int i = 0; i < 6; i++)
            {
                sixNumbers.Add(random.Next(recipeListCount.Count()));
            }
            return sixNumbers;
        }
    }
}

