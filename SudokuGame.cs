using sudokuSolver.ObjectRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UiPath.Core;
using UiPath.Core.Activities.Storage;
using UiPath.Excel;
using UiPath.Excel.Activities;
using UiPath.Excel.Activities.API;
using UiPath.Excel.Activities.API.Models;
using UiPath.Mail.Activities.Api;
using UiPath.Orchestrator.Client.Models;
using UiPath.Testing;
using UiPath.Testing.Activities.Api.Models;
using UiPath.Testing.Activities.Models;
using UiPath.Testing.Activities.TestData;
using UiPath.Testing.Activities.TestDataQueues.Enums;
using UiPath.Testing.Enums;
using UiPath.UIAutomationNext.API.Contracts;
using UiPath.UIAutomationNext.API.Models;
using UiPath.UIAutomationNext.Enums;

namespace sudokuSolver
{
    public class SudokuGame
    {
        public List<List<UiElement>> cellElements = new();
        public List<List<string>> globalMatrixSudoku = new();
        public List<coordinates> emptyCells = new();
        public HashSet<string> cellOptions = "1,2,3,4,5,6,7,8,9".Split(",").ToHashSet();
        public int globalTotalEmtpy=0;
        public List<List<string>> originalMatrixSudoku = new();
        
        public SudokuGame(List<List<UiElement>> nCellElements){
            cellElements=nCellElements;
            generateListValues();
            getEmtpyCells();
        }
        
        
        public void generateListValues(){
            foreach(int i in Enumerable.Range(0,9)){
                List<string> newRow = new();
                foreach(int j in Enumerable.Range(0,9)){
                    string cellValue = cellElements[i][j].Get("innerText").ToString();
                    if(cellValue=="") globalTotalEmtpy++;
                    newRow.Add(cellElements[i][j].Get("innerText").ToString());
                }
                globalMatrixSudoku.Add(newRow);
            }
            originalMatrixSudoku = cloneMatrix(globalMatrixSudoku);
        }
        
        
        
    public void getEmtpyCells(){
        foreach(int i in Enumerable.Range(0,9)){
            foreach(int j in Enumerable.Range(0,9)){
                if(globalMatrixSudoku[i][j]==""){
                    emptyCells.Add(new coordinates(i,j));
                }
        }
        }
    }
    
    public coordinates findCellBox(coordinates cell){
        coordinates box = new(Convert.ToInt16(Math.Floor(cell.x/3.0)),Convert.ToInt16(Math.Floor(cell.y/3.0)));
        return box;
    }
    
   
    
    public (List<List<string>>,int) checkCellsToFill(List<List<string>> matrixSudoku, int totalEmpty){
        if(totalEmpty==0) return (matrixSudoku,totalEmpty);
        coordinates cell=null;
        cell= checkRowsByMultipleCell(matrixSudoku) ?? checkColumnsByMultipleCell(matrixSudoku) ?? checkMatrixByMultipleCell(matrixSudoku);
        totalEmpty--;
        if(cell==null){
           (coordinates coord, HashSet<string> currentCellOpts) = findBestCell(matrixSudoku);
            foreach(string opt in currentCellOpts){
                var newMatrix = cloneMatrix(matrixSudoku);
                newMatrix[coord.x][coord.y]=opt;
                (List<List<string>> possibleSolution, int currentTotal) =checkCellsToFill(newMatrix,totalEmpty);
                if(currentTotal==0){
                return (possibleSolution, currentTotal);    
                }
            }
        }
        else{
            matrixSudoku[cell.x][cell.y]=cell.value;
            return checkCellsToFill(cloneMatrix(matrixSudoku),totalEmpty);
        }
        return (null,-1);
    }
    
    public void findSolution(){
        (globalMatrixSudoku,globalTotalEmtpy)=checkCellsToFill(cloneMatrix(globalMatrixSudoku), globalTotalEmtpy);
    }
    
    public List<List<string>> cloneMatrix(List<List<string>> currentMatrix){
        List<List<string>> newMatrix = new();
        for(int i=0; i<9; i++){
            newMatrix.Add(new List<string>());
            for(int j=0; j<9; j++){
               newMatrix[i].Add(currentMatrix[i][j]);
            }
        }
        return newMatrix;
    }
    
    
    public (coordinates, HashSet<string>) findBestCell(List<List<string>> matrixSudoku){
        coordinates bestCoordinate = new coordinates();
        HashSet<string> options=new HashSet<string>(cellOptions);
        for(int i=0; i<9;i++){
            for(int j=0;j<9;j++){
                if(matrixSudoku[i][j]==""){
                    var currentCoordinate = new coordinates(i,j);
                    var currentCellOptions=cellOptions.Where((string number)=> !checkIfNumberInColumnOrMatrixOrRow(number, currentCoordinate,matrixSudoku));
                    if(currentCellOptions.Count()<options.Count()){
                        bestCoordinate=currentCoordinate;
                        options=currentCellOptions.ToHashSet();
                    }
                }
            }
        }
        return (bestCoordinate,options);
    }
    
    
    
    public coordinates checkRowsByMultipleCell(List<List<string>> matrixSudoku){
            for(int j=0; j<9;j++){
            coordinates elementFound = MethodRowMultipleCell(j, matrixSudoku);
                if(elementFound!=null){
                    return elementFound;
                }
        }
        return null;
    }
    
    public coordinates checkColumnsByMultipleCell(List<List<string>> matrixSudoku){
            for(int i=0; i<9;i++){
            coordinates elementFound = MethodColumnMultipleCell(i,matrixSudoku);
                if(elementFound!=null){
                    return elementFound;
                }
        }
        return null;
    }
    
    public coordinates checkMatrixByMultipleCell(List<List<string>> matrixSudoku){
        for(int i=0; i<3;i++){
            for(int j=0; j<3;j++){
                coordinates matrixBox = new coordinates(i,j);
            coordinates elementFound = MethodMatrixMultipleCells(matrixBox,matrixSudoku);
                if(elementFound!=null){
                    return elementFound;
                }
        }
        }
        return null;
    }
    
    
    public coordinates MethodMatrixMultipleCells(coordinates matrixBox, List<List<string>> matrixSudoku){
        (List<coordinates> emptyCells, HashSet<string> cellOpts) = getEmptyMatrixCells(matrixBox,matrixSudoku);
        
        foreach(coordinates emptyCell in emptyCells){
            HashSet<string> opts = new HashSet<string>(cellOpts);
              foreach(string cellOpt in cellOpts){
                  if(checkIfNumberInRowOrColumns(cellOpt,emptyCell,matrixSudoku)){
                      opts.Remove(cellOpt);
                  }
        }
            if(opts.Count()==1){
                emptyCell.value=opts.ToList()[0];
                return emptyCell;
            }
        }
        return null;
    }
    
    public coordinates MethodColumnMultipleCell(int col, List<List<string>> matrixSudoku){
        (List<coordinates> emptyCells, HashSet<string> cellOpts) = getEmptyColumnCells(col, matrixSudoku);
        foreach(coordinates emptyCell in emptyCells){
            HashSet<string> opts = new HashSet<string>(cellOpts);
              foreach(string cellOpt in cellOpts){
                  if(checkIfNumberInRowOrMatrix(cellOpt,emptyCell, matrixSudoku)){
                      opts.Remove(cellOpt);
                  }
        }
            if(opts.Count()==1){
                emptyCell.value=opts.ToList()[0];
                return emptyCell;
            }
        }
        return null;
    }
    
    public coordinates MethodRowMultipleCell(int row, List<List<string>> matrixSudoku){
        (List<coordinates> emptyCells, HashSet<string> cellOpts) = getEmptyRowCells(row, matrixSudoku);
        
        foreach(coordinates emptyCell in emptyCells){
            HashSet<string> opts = new HashSet<string>(cellOpts);
              foreach(string cellOpt in cellOpts){
                  if(checkIfNumberInColumnOrMatrix(cellOpt,emptyCell, matrixSudoku)){
                      opts.Remove(cellOpt);
                  }
        }
            if(opts.Count()==1){
                emptyCell.value=opts.ToList()[0];
                return emptyCell;
            }
        }
        return null;
    }
    
    public bool checkIfNumberInColumnOrMatrixOrRow(string val, coordinates cell, List<List<string>> matrixSudoku){
        return CheckIfNumberInColumn(val, cell, matrixSudoku)  || CheckIfNumberInMatrix(val, cell, matrixSudoku) || CheckIfNumberInRow(val, cell, matrixSudoku);
    }
    
    public bool checkIfNumberInColumnOrMatrix(string val, coordinates cell, List<List<string>> matrixSudoku){
        return CheckIfNumberInColumn(val, cell, matrixSudoku)  || CheckIfNumberInMatrix(val, cell, matrixSudoku);
    }
    
    public bool checkIfNumberInRowOrMatrix(string val, coordinates cell, List<List<string>> matrixSudoku){
        return CheckIfNumberInRow(val, cell, matrixSudoku)  || CheckIfNumberInMatrix(val, cell, matrixSudoku);
    }
    
    public bool checkIfNumberInRowOrColumns(string val, coordinates cell, List<List<string>> matrixSudoku){
        return CheckIfNumberInRow(val, cell, matrixSudoku)  || CheckIfNumberInColumn(val, cell, matrixSudoku);
    }
    
    public bool CheckIfNumberInColumn(string val, coordinates cell, List<List<string>> matrixSudoku){
        for(int j=0; j<9;j++){
            if(matrixSudoku[j][cell.y]==val){
                return true;
            }
        }
        return false;
    }
    
    public bool CheckIfNumberInRow(string val, coordinates cell, List<List<string>> matrixSudoku){
        for(int i=0; i<9;i++){
            if(matrixSudoku[cell.x][i]==val){
                return true;
            }
        }
        return false;
    }
    
    public bool CheckIfNumberInMatrix(string val, coordinates cell, List<List<string>> matrixSudoku){
        coordinates matrixBox = new coordinates(Convert.ToInt16(Math.Floor(cell.x/3.0))*3,Convert.ToInt16(Math.Floor(cell.y/3.0))*3);
        for(int i=0;i<3;i++){
            for(int j=0;j<3;j++){
                if(matrixSudoku[matrixBox.x+i][matrixBox.y+j]==val){
                return true;
            }
            }
        }
        return false;
    }
    

    public (List<coordinates>, HashSet<string>) getEmptyRowCells(int i, List<List<string>> matrixSudoku){
        HashSet<string> cellOptions_ = new HashSet<string>(cellOptions);
        List<coordinates>emptyCells = new(); 
            foreach(int j in Enumerable.Range(0,9)){
            if(matrixSudoku[i][j]==""){
                emptyCells.Add(new coordinates(i,j));
            }
            else{
                cellOptions_.Remove(matrixSudoku[i][j]);

        }
        }
        return (emptyCells, cellOptions_);
    }
    
    public (List<coordinates>, HashSet<string>) getEmptyColumnCells(int j, List<List<string>> matrixSudoku){
        HashSet<string> cellOptions_ = new HashSet<string>(cellOptions);
        List<coordinates>emptyCells = new(); 
            foreach(int i in Enumerable.Range(0,9)){
            if(matrixSudoku[i][j]==""){
                emptyCells.Add(new coordinates(i,j));
            }
            else{
                cellOptions_.Remove(matrixSudoku[i][j]);

        }
        }
        return (emptyCells, cellOptions_);
    }
    
    public (List<coordinates>, HashSet<string>) getEmptyMatrixCells(coordinates box, List<List<string>> matrixSudoku){
        HashSet<string> cellOptions_ = new HashSet<string>(cellOptions);
        int initialX = box.x*3;
        int initialY = box.y*3;
        List<coordinates>emptyCells = new(); 
        foreach(int i in Enumerable.Range(0,3)){
            foreach(int j in Enumerable.Range(0,3)){
            if(matrixSudoku[initialX+i][initialY+j]==""){
                emptyCells.Add(new coordinates(initialX+i,initialY+j));
            }
            else{
                cellOptions_.Remove(matrixSudoku[initialX+i][initialY+j]);
            }
        }
        }
        return (emptyCells, cellOptions_);
    }
    
    }
}