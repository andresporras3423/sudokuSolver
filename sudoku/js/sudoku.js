(function( $ ){

  var methods = {
     init : function( options ) {

		return this.each(function() {  
			var settings = {
				levels : [
					{level: "Easy"},
					{level: "Medium"},
					{level: "Hard"}
				]
			};

			var defaults = {
				matrix : [],
				domMatrix : [],
				numOfRows : 9,
				numOfCols : 9,
				level : 40,
				selected : null,
				selectedSolution : null,
				anwerTracker : {
					"1" : 9,
					"2" : 9,
					"3" : 9,
					"4" : 9,
					"5" : 9,
					"6" : 9,
					"7" : 9,
					"8" : 9,
					"9" : 9
				}
			}
      		if ( options ) {
      			$.extend( settings, options );
      		}

			var $this = $(this);
			$this.addClass('sdk-game');
			
			

			$this.fetchSudokuPuzzle = async function(level) {
				const url = 'https://sudoku-api.vercel.app/api/dosuku?level='+level;
				
				try {
				  // Fetch data from the endpoint
				  const response = await fetch(url);
				  
				  // Check if the response is ok (status 200)
				  if (!response.ok) {
					throw new Error(`HTTP error! status: ${response.status}`);
				  }
				  
				  // Parse the JSON response
				  const data = await response.json();
				  const  values = data["newboard"]["grids"][0]["value"];

				  for(var row=0;row<9;row++){
					for(var col=0;col<9;col++){
						if(values[row][col]!="0")
						{
							defaults.domMatrix[row][col].append("<div class='sdk-solution'>"+ values[row][col] +"</div>");
							defaults.anwerTracker[values[row][col]]--;
						}
					}
				  }
				  defaults.matrix=data["newboard"]["grids"][0]["solution"];
				  // Log or process the data
				  console.log('Sudoku puzzle data:', data);
				  return data; // Return the data for further processing
				} catch (error) {
				  console.error('Error fetching Sudoku puzzle:', error);
				}
			  }
			  
			  
			
			// create the playable table
			$this.createTable = function() {
				//array to hold the dom reference to the table matrix so that we dont have to travers dom all the time
				defaults.domMatrix = [];
				//create table 
				defaults.table = $("<div class='sdk-table sdk-no-show'></div>");
				//add rows and columns to table
				for (var row=0;row<defaults.numOfRows;row++) {
					defaults.domMatrix[row] = [];
					var tempRow = $("<div class='sdk-row'></div>");
					//set solid border after 3rd and 6th row
					if (row == 2 || row == 5) tempRow.addClass("sdk-border"); 
					for (var col=0;col<defaults.numOfCols;col++) {
						defaults.domMatrix[row][col] = $("<div class='sdk-col' data-row='"+row+"' data-col='"+col+"'></div>");
						//set solid border after 3rd and 6th column
						if (col == 2 || col == 5) defaults.domMatrix[row][col].addClass("sdk-border");
						//add columns to rows
						tempRow.append(defaults.domMatrix[row][col]);
					}
					//add rows to table
					defaults.table.append(tempRow);
				}
				//add extra div in here for background decoration
				defaults.table.append("<div class='sdk-table-bk'></div>");
				//add table to screen
				$this.append(defaults.table);
				
				//populate table with random number depending on the level difficulty 
				var level = defaults.level;
				// Example usage
				$this.fetchSudokuPuzzle(level);
				// while (items > 0) {
				// 	var row = Math.floor(Math.random() * (8 - 0 + 1)) + 0;
				// 	var col = Math.floor(Math.random() * (8 - 0 + 1)) + 0;
				// 	if (defaults.domMatrix[row][col].children().length == 0) {
				// 		defaults.domMatrix[row][col].append("<div class='sdk-solution'>"+ defaults.matrix[row][col] +"</div>");
				// 		defaults.anwerTracker[defaults.matrix[row][col].toString()]--;
				// 		items--;
				// 	}
				// }
				//click event when clicking on cells
				defaults.table.find(".sdk-col").click(function () {
					//remove any helper styling
					$this.find(".sdk-solution").removeClass("sdk-helper");
					$this.find(".sdk-col").removeClass("sdk-selected");
					if ($(this).children().length == 0) {
						//select this 
						defaults.domMatrix[$(this).attr("data-row")][$(this).attr("data-col")].addClass("sdk-selected");
						defaults.selected = defaults.domMatrix[$(this).attr("data-row")][$(this).attr("data-col")];
						defaults.selectedSolution = defaults.matrix[$(this).attr("data-row")][$(this).attr("data-col")];
					} else {
						//add helper style
						$this.highlightHelp(parseInt($(this).text()));
					}
				});
				
				//add answers choices to screen
				$this.answerPicker();
								
				//remove the no show class to do a small fadein animation with css
				setTimeout(function () {
					defaults.table.removeClass("sdk-no-show");
				}, 300);
			};
			
			//add answer picker to screen
			$this.answerPicker = function() {
				//make a answer container 
				var answerContainer = $("<div class='sdk-ans-container'></div>");
				//add answer buttons to container
				for (var a in defaults.anwerTracker) {
					//check if need to show button else we add it for space reason but dont pick up clicks from it
					if (defaults.anwerTracker[a] > 0) {
						answerContainer.append("<div class='sdk-btn'>"+a+"</div>");
					} else {
						answerContainer.append("<div class='sdk-btn sdk-no-show'>"+a+"</div>");
					}
				}
				answerContainer.find(".sdk-btn").click(function () {
					//only listen to clicks if it is shown
					if (!$(this).hasClass("sdk-no-show") && defaults.selected != null && defaults.selected.children().length == 0 ) {
						//check if it is the answer
						if ( defaults.selectedSolution == parseInt($(this).text()) ) {
							//decrease answer tracker
							defaults.anwerTracker[$(this).text()]--;
							//if answer tracker is 0 hide that button
							if (defaults.anwerTracker[$(this).text()] == 0) {
								$(this).addClass("sdk-no-show");
							}
							//remove highlighter
							$this.find(".sdk-col").removeClass("sdk-selected");
							//add the answer to screen
							defaults.selected.append("<div class='sdk-solution'>"+ defaults.selectedSolution +"</div>");
						}
						
					}
				});
				$this.append(answerContainer);
				
			};
			
			//add highlight help
			$this.highlightHelp = function(number) {
				//loop through dom matrix to find filled in number that match the number we clicked on
				for (var row=0;row<defaults.numOfRows;row++) {
					for (var col=0;col<defaults.numOfCols;col++) {
						if ( parseInt(defaults.domMatrix[row][col].text()) == number ) {
							defaults.domMatrix[row][col].find(".sdk-solution").addClass("sdk-helper");
						}
					}
				}
			};
			
			// create difficulty picker 
			$this.createDiffPicker = function() {
				//level picker container
				var picker = $("<div class='sdk-picker sdk-no-show'></div>");
				//loop through all levels possible and add buttons to the picker container
				$(settings.levels).each(function (e) {
					picker.append("<div class='sdk-btn' data-level='"+this.level+"'>"+this.level+"</div>");
				});
				//add it to screen
				$this.append(picker);
				//click event for the level select buttons
				picker.find(".sdk-btn").click(function () {
					picker.addClass("sdk-no-show");
					defaults.level = $(this).attr("data-level");
					//wait for animation to complete to continue on
					setTimeout(function () {
						// remove the picker from the DOM
						picker.remove();
						// add the playable table to screen. 
						$this.createTable();
					}, 2000);
				});
				//remove the no show class to do a small fadein animation with css
				setTimeout(function () {
					picker.removeClass("sdk-no-show");
				}, 500);
			};
			
			$this.createDiffPicker();
			
      			
     	});
     }
  };   	 	

  $.fn.sudoku = function( method ) {
    
    if ( methods[method] ) {
      return methods[method].apply( this, Array.prototype.slice.call( arguments, 1 ));
    } else if ( typeof method === 'object' || ! method ) {
      return methods.init.apply( this, arguments );
    } else {
      $.error( 'Method ' +  method + ' does not exist on jQuery.sudoku' );
    }    
  
  };

})( jQuery );