﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsCookinGroupCapstone.Contracts;
using WhatsCookinGroupCapstone.Data;
using static WhatsCookinGroupCapstone.Contracts.IRepositoryBase;

namespace WhatsCookinGroupCapstone
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private ApplicationDbContext _context;
        private ICookRepository _cook;
        private IFollowersRepository _followers;
        private IRecipeRepository _recipe;
        private IRecipeTagsRepository _recipeTags;
        private IReviewsRepository _reviews;
        private ITagsRepository _tags;
        private ICookTagRepository _cookTag;
        private ICookSavedRecipesRepository _cookSavedRecipes;
        private IRecipeEditsRepository _recipeEdits;
        public ICookRepository Cook
        {
            get
            {
                if (_cook == null)
                {
                    _cook = new CookRepository(_context);
                }
                return _cook;
            }
        }
        public IFollowersRepository Followers
        {
            get
            {
                if (_followers == null)
                {
                    _followers = new FollowersRepository(_context);
                }
                return _followers;
            }
        }
        public IRecipeRepository Recipe
        {
            get
            {
                if (_recipe == null)
                {
                    _recipe = new RecipeRepository(_context);
                }
                return _recipe;
            }
        }
        public IRecipeTagsRepository RecipeTags
        {
            get
            {
                if (_recipeTags == null)
                {
                    _recipeTags = new RecipeTagsRepository(_context);
                }
                return _recipeTags;
            }
        }
        public IReviewsRepository Reviews
        {
            get
            {
                if (_reviews == null)
                {
                    _reviews = new ReviewsRepository(_context);
                }
                return _reviews;
            }
        }
        public ITagsRepository Tags
        {
            get
            {
                if (_tags == null)
                {
                    _tags = new TagsRepository(_context);
                }
                return _tags;
            }
        }
        public ICookTagRepository CookTag
        {
            get
            {
                if (_cookTag == null)
                {
                    _cookTag = new CookTagRepository(_context);
                }
                return _cookTag;
            }
        }
        public ICookSavedRecipesRepository CookSavedRecipes
        {
            get
            {
                if (_cookSavedRecipes == null)
                {
                    _cookSavedRecipes = new CookSavedRecipesRepository(_context);
                }
                return _cookSavedRecipes;
            }
        }

        public IRecipeEditsRepository RecipeEdits
        {
            get
            {
                if(_recipeEdits == null)
                {
                    _recipeEdits = new RecipeEditsRepository(_context);
                }
                return _recipeEdits;
            }
        }


        public RepositoryWrapper(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
