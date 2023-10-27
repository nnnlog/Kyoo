// Kyoo - A portable and vast media library solution.
// Copyright (c) Kyoo.
//
// See AUTHORS.md and LICENSE file in the project root for full license information.
//
// Kyoo is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// Kyoo is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Kyoo. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kyoo.Abstractions.Models;
using Kyoo.Abstractions.Models.Exceptions;
using Kyoo.Abstractions.Models.Utils;

namespace Kyoo.Abstractions.Controllers
{
	/// <summary>
	/// A common repository for every resources.
	/// </summary>
	/// <typeparam name="T">The resource's type that this repository manage.</typeparam>
	public interface IRepository<T> : IBaseRepository
		where T : class, IResource
	{
		/// <summary>
		/// The event handler type for all events of this repository.
		/// </summary>
		/// <param name="resource">The resource created/modified/deleted</param>
		public delegate void ResourceEventHandler(T resource);

		/// <summary>
		/// Get a resource from it's ID.
		/// </summary>
		/// <param name="id">The id of the resource</param>
		/// <param name="include">The related fields to include.</param>
		/// <exception cref="ItemNotFoundException">If the item could not be found.</exception>
		/// <returns>The resource found</returns>
		Task<T> Get(int id, Include<T>? include = default);

		/// <summary>
		/// Get a resource from it's slug.
		/// </summary>
		/// <param name="slug">The slug of the resource</param>
		/// <param name="include">The related fields to include.</param>
		/// <exception cref="ItemNotFoundException">If the item could not be found.</exception>
		/// <returns>The resource found</returns>
		Task<T> Get(string slug, Include<T>? include = default);

		/// <summary>
		/// Get the first resource that match the predicate.
		/// </summary>
		/// <param name="where">A predicate to filter the resource.</param>
		/// <param name="include">The related fields to include.</param>
		/// <exception cref="ItemNotFoundException">If the item could not be found.</exception>
		/// <returns>The resource found</returns>
		Task<T> Get(Expression<Func<T, bool>> where, Include<T>? include = default);

		/// <summary>
		/// Get a resource from it's ID or null if it is not found.
		/// </summary>
		/// <param name="id">The id of the resource</param>
		/// <param name="include">The related fields to include.</param>
		/// <returns>The resource found</returns>
		Task<T?> GetOrDefault(int id, Include<T>? include = default);

		/// <summary>
		/// Get a resource from it's slug or null if it is not found.
		/// </summary>
		/// <param name="slug">The slug of the resource</param>
		/// <param name="include">The related fields to include.</param>
		/// <returns>The resource found</returns>
		Task<T?> GetOrDefault(string slug, Include<T>? include = default);

		/// <summary>
		/// Get the first resource that match the predicate or null if it is not found.
		/// </summary>
		/// <param name="where">A predicate to filter the resource.</param>
		/// <param name="include">The related fields to include.</param>
		/// <param name="sortBy">A custom sort method to handle cases where multiples items match the filters.</param>
		/// <returns>The resource found</returns>
		Task<T?> GetOrDefault(Expression<Func<T, bool>> where,
			Include<T>? include = default,
			Sort<T>? sortBy = default);

		/// <summary>
		/// Search for resources.
		/// </summary>
		/// <param name="query">The query string.</param>
		/// <returns>A list of resources found</returns>
		Task<ICollection<T>> Search(string query);

		/// <summary>
		/// Get every resources that match all filters
		/// </summary>
		/// <param name="where">A filter predicate</param>
		/// <param name="sort">Sort information about the query (sort by, sort order)</param>
		/// <param name="limit">How pagination should be done (where to start and how many to return)</param>
		/// <param name="include">The related fields to include.</param>
		/// <returns>A list of resources that match every filters</returns>
		Task<ICollection<T>> GetAll(Expression<Func<T, bool>>? where = null,
			Sort<T>? sort = default,
			Pagination? limit = default,
			Include<T>? include = default);

		/// <summary>
		/// Get the number of resources that match the filter's predicate.
		/// </summary>
		/// <param name="where">A filter predicate</param>
		/// <returns>How many resources matched that filter</returns>
		Task<int> GetCount(Expression<Func<T, bool>>? where = null);

		/// <summary>
		/// Create a new resource.
		/// </summary>
		/// <param name="obj">The item to register</param>
		/// <returns>The resource registers and completed by database's information (related items and so on)</returns>
		Task<T> Create(T obj);

		/// <summary>
		/// Create a new resource if it does not exist already. If it does, the existing value is returned instead.
		/// </summary>
		/// <param name="obj">The object to create</param>
		/// <returns>The newly created item or the existing value if it existed.</returns>
		Task<T> CreateIfNotExists(T obj);

		/// <summary>
		/// Called when a resource has been created.
		/// </summary>
		event ResourceEventHandler OnCreated;

		/// <summary>
		/// Edit a resource and replace every property
		/// </summary>
		/// <param name="edited">The resource to edit, it's ID can't change.</param>
		/// <exception cref="ItemNotFoundException">If the item is not found</exception>
		/// <returns>The resource edited and completed by database's information (related items and so on)</returns>
		Task<T> Edit(T edited);

		/// <summary>
		/// Edit only specific properties of a resource
		/// </summary>
		/// <param name="id">The id of the resource to edit</param>
		/// <param name="patch">
		/// A method that will be called when you need to update every properties that you want to
		/// persist. It can return false to abort the process via an ArgumentException
		/// </param>
		/// <exception cref="ItemNotFoundException">If the item is not found</exception>
		/// <returns>The resource edited and completed by database's information (related items and so on)</returns>
		Task<T> Patch(int id, Func<T, Task<bool>> patch);

		/// <summary>
		/// Called when a resource has been edited.
		/// </summary>
		event ResourceEventHandler OnEdited;

		/// <summary>
		/// Delete a resource by it's ID
		/// </summary>
		/// <param name="id">The ID of the resource</param>
		/// <exception cref="ItemNotFoundException">If the item is not found</exception>
		/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
		Task Delete(int id);

		/// <summary>
		/// Delete a resource by it's slug
		/// </summary>
		/// <param name="slug">The slug of the resource</param>
		/// <exception cref="ItemNotFoundException">If the item is not found</exception>
		/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
		Task Delete(string slug);

		/// <summary>
		/// Delete a resource
		/// </summary>
		/// <param name="obj">The resource to delete</param>
		/// <exception cref="ItemNotFoundException">If the item is not found</exception>
		/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
		Task Delete(T obj);

		/// <summary>
		/// Delete all resources that match the predicate.
		/// </summary>
		/// <param name="where">A predicate to filter resources to delete. Every resource that match this will be deleted.</param>
		/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
		Task DeleteAll(Expression<Func<T, bool>> where);

		/// <summary>
		/// Called when a resource has been edited.
		/// </summary>
		event ResourceEventHandler OnDeleted;
	}

	/// <summary>
	/// A base class for repositories. Every service implementing this will be handled by the <see cref="ILibraryManager"/>.
	/// </summary>
	public interface IBaseRepository
	{
		/// <summary>
		/// The type for witch this repository is responsible or null if non applicable.
		/// </summary>
		Type RepositoryType { get; }
	}
}
