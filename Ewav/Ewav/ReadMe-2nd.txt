Second ReadMe for Simple MVVM with WCF RIA Services

After adding a data model and a domain service to the Web project, code will
be generated in the linked client project when the Web project is built.

You can either modify or replace the existing classes in Services, ViewModels
and Views.

1. Using IItemListServiceAgent as an example, create a service agent interface
   with methods for retrieving, adding and removing entities, as well as methods
   to save changes and reject changes.

2. Using ItemListServiceAgent as an example, add a class that implementes the
   service interface methods you defined in the first step.
   - Flesh out retrieve methods
     > Load a domain context query with a callback that provides entities or an error
   - Flesh out Add and Remove methods
     > Call Add or Remove on domain context entity sets
   - Flesh out SaveChanges
     > If the entity set has changes, call SubmitChanges on the domain context,
	   checking for an error and invoking the completion callback method
   - Flesh out RejectChanges
     > If entity set has changes, call RejectChanges on the domain context
   - Lastly, make sure to decorate the service agent class with a ServiceAgentExport
     attribute, specifying an AgentType of Real.

3. Add a ViewModel to the project by adding a new item and selecting SimpleMvvmViewModel
   under the Silverlight / Mvvm category.
   - Using ItemListViewModel as an example, add a property of type ObservableCollection<T>,
     as well as SelectedItem and IsBusy, CanLoad, CanAdd, CanRemove, CanEdit properties
   - Add a method to load items with a callback method to handle completion
     > In the completion method set the items property or notify View of an error
   - Also insert Add, Remove, SaveChanges and RejectChanges methods
     > These methods use the service agent to perform the operations

4. Open ViewModelLocator.cs and, using mvvminjectedlocator snippets add the ViewModel
   created in the previous step.
   - Uncomment and move the Debug.Assert code to the ctor to verify service agent creation
   - Lastly, be sure to change the AgentType property to default to Real instead of Mock

5. Add a Silverlight Page to the Views folder appending View to the end of the name
   - Using ItemListView as an example, set the DataContext to the Locator static resource,
     specifying the ViewModel name for the Path.
   - In the ctor in the View code-behind, obtain a reference to the ViewModel from
     the DataContext
	 > Then subscribe to receive notifications from the ViewModel
	 > By default you can handle notifications for errors and saving changes
   - Optionally handle click events for buttons to add, edit or remove items
   - Enable saving changes and rejecting changes

6. Lastly, using ItemViewModel as an example, you can add a ViewModel for a dialog
   to add or edit individual items.
   - Add a new item to the ViewModels folder, selecting SimpleMvvmViewModelDetail
     from the Silverlight / Mvvm category
   - If there is no need for a service agent, you can remove those sections of the code
   - Supply a detail type for the ctor sets the Model property
   - Using the mvvmlocatornosa code snippet, add the ViewModel to the view model locator
   - Using ItemDetailView as an example, add a Silverlight Child Window to the Views folder
     > In the code behind you can add a ctor that accepts a ViewModel to set the View's
	   DataContext (the code-behind for the parent View can supply the ViewModel).

  You should be able to build and run the Silverlight client project.

