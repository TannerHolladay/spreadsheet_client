// Written by Tanner Holladay for CS 3500, September 2020.
// Updated by Noah Carlson for CS 3505, April 28, 2021

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using SpreadsheetUtilities;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        // All cells to Cell objects. Cells all contain contents and a value.
        private readonly Dictionary<string, Cell> _cells;

        // All the dependency pairs of the spreadsheet where formulas depend on other cells
        private readonly DependencyGraph _dependencies;

        /// <summary>
        ///     Creates a spreadsheet of infinite cells that can have formulas, numbers, and strings.
        ///     Formulas will calculate equations using the "=" symbol. Formulas can use other cells in the equation
        ///     as long as their value returns a number. All cells require one or more letters followed by one or more
        ///     numbers
        ///     Version will be set to "default" when using no parameters
        /// </summary>
        public Spreadsheet() : this(s => true, n => n, "default")
        {
        }

        /// <summary>
        ///     Creates a spreadsheet of infinite cells that can have formulas, numbers, and strings.
        ///     Formulas will calculate equations using the "=" symbol. Formulas can use other cells in the equation
        ///     as long as their value returns a number. All cells require one or more letters followed by one or more
        ///     numbers
        /// </summary>
        /// <param name="isValid">Method that used to determine if a cell is valid along with the base validity</param>
        /// <param name="normalize">
        ///     Method used to convert input cell names to something else. (Likely used to convert to upper or
        ///     lower)
        /// </param>
        /// <param name="version">The version to be set for the spreadsheet</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version ?? "default")
        {
            IsValid = valid => isValid(valid = Normalize(valid)) && Regex.IsMatch(valid, @"^[a-zA-Z]+(\d)*$");
            _cells = new Dictionary<string, Cell>();
            _dependencies = new DependencyGraph();
        }

        /// <summary>
        ///     Creates a spreadsheet of infinite cells that can have formulas, numbers, and strings.
        ///     Formulas will calculate equations using the "=" symbol. Formulas can use other cells in the equation
        ///     as long as their value returns a number. All cells require one or more letters followed by one or more
        ///     numbers
        ///     This will load a previously created spreadsheet.
        /// </summary>
        /// <param name="filename">The file to load the spreadsheet from</param>
        /// <param name="isValid">Method that used to determine if a cell is valid along with the base validity</param>
        /// <param name="normalize">
        ///     Method used to convert input cell names to something else. (Likely used to convert to upper or
        ///     lower)
        /// </param>
        /// <param name="version">The version to be set for the spreadsheet</param>
        public Spreadsheet(string filename, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : this(isValid, normalize, version)
        {
            // Reads the contents of the file using XmlReader
            // If anything is thrown while reading, then code will throw a SpreadsheetReadWriteException
            if (!File.Exists(filename)) throw new SpreadsheetReadWriteException("Filepath given is invalid");
            try
            {
                using (var reader = XmlReader.Create(filename))
                {
                    reader.Read();
                    reader.ReadToFollowing("spreadsheet");
                    if (reader.GetAttribute("version") != version)
                        throw new Exception("Spreadsheet version does not match");
                    while (reader.Read())
                    {
                        if (!reader.IsStartElement() || reader.Name != "cell") continue;
                        if (!reader.ReadToDescendant("name"))
                            throw new Exception("Name of cell does not exist");
                        string name = reader.ReadElementString("name");
                        if (reader.Name != "contents" && !reader.ReadToNextSibling("contents"))
                            throw new Exception("Cell contents does not exist");
                        string contents = reader.ReadElementString("contents");
                        SetContentsOfCell(name, contents);
                    }
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(
                    $"Error received when trying to load the Spreadsheet! {e.Message}");
            }

            Changed = false;
        }

        /// <summary>
        ///     Returns true if the spreadsheet has been changed since last edited, false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        ///     Gets the version from the filename specified
        /// </summary>
        /// <param name="filename">Path of the file to read</param>
        /// <returns>Returns the version of the spreadsheet file</returns>
        public override string GetSavedVersion(string filename)
        {
            if (!File.Exists(filename)) throw new SpreadsheetReadWriteException("File given does not exist");
            try
            {
                using (var reader = XmlReader.Create(filename))
                {
                    reader.Read();
                    reader.ReadToFollowing("spreadsheet");
                    if (!reader.MoveToAttribute("version")) throw new Exception("Version does not exist");
                    return reader.Value;
                }
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException(
                    "Error received when attempting to read the spreadsheet version!");
            }
        }

        /// <summary>
        ///     Saves the spreadsheet to a file with the specified filename
        /// </summary>
        /// <param name="filename">The name of the file</param>
        public override void Save(string filename)
        {
            try
            {
                using (var writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    foreach (var cell in _cells)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", cell.Key);
                        object contents = cell.Value.Contents;
                        writer.WriteElementString("contents", $"{(contents is Formula ? "=" : "")}{contents}");
                        writer.WriteEndElement();
                    }


                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                Changed = false;
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Error was thrown when trying to save spreadsheet to file!");
            }
        }

        /// <summary>
        ///     Gets the calculated value of the cell with the given name
        /// </summary>
        /// <param name="name">The name of the cell</param>
        /// <returns>The calculated result</returns>
        public override object GetCellValue(string name)
        {
            if (name is null || !IsValid(name = Normalize(name))) throw new InvalidNameException();
            return _cells.TryGetValue(name, out var value) ? value.Value : "";
        }

        /// <summary>
        ///     Returns an IEnumerable of the names of cells that contain contents
        /// </summary>
        /// <returns>The name of cells with contents</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return _cells.Keys;
        }

        /// <summary>
        ///     Returns the raw contents of a cell
        /// </summary>
        /// <param name="name">The name of the cell to get the contents from</param>
        /// <returns>The object contained in the cell</returns>
        public override object GetCellContents(string name)
        {
            if (name is null || !IsValid(name = Normalize(name))) throw new InvalidNameException();

            return _cells.TryGetValue(name, out var value) ? value.Contents : "";
        }

        /// <summary>
        ///     Sets the contents of cell. Contents will be either a formula, number, or just text.
        ///     Formulas can be defined using an equal("=") symbol before the equation.
        /// </summary>
        /// <param name="name">The name of the cell to change the contents</param>
        /// <param name="content">The contents to be inserted into the cell</param>
        /// <returns>Returns the direct and indirect dependence's</returns>
        public sealed override IList<string> SetContentsOfCell(string name, string content)
        {
            if (content is null) throw new ArgumentNullException(nameof(content), "Content cannot be null");
            if (name is null || !IsValid(name = Normalize(name))) throw new InvalidNameException();

            IList<string> cells;
            if (content != "" && content[0] == '=')
                cells = SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
            else if (double.TryParse(content, out double number))
                cells = SetCellContents(name, number);
            else
                cells = SetCellContents(name, content);

            foreach (string cell in cells)
            {
                if (!_cells.TryGetValue(cell, out var value)) continue;
                // This updates all cells that depend on the updated cell. Throws an error if cell in equation is not a number
                value.Evaluate(n =>
                {
                    if (GetCellValue(n) is double number) return number;
                    throw new ArgumentException("Variable in equation is not a number");
                });
            }

            Changed = true;
            return cells;
        }

        /// <summary>
        ///     Sets the contents of a cell to be a number
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <param name="number">Number to be passed into the contents</param>
        /// <returns>Returns all the direct and indirect cells that depend on this cell</returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            _dependencies.ReplaceDependees(name, null);
            _cells[name] = new Cell(number);
            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        ///     Sets the contents of a cell to be some text
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <param name="text">Text to be passed into the contents</param>
        /// <returns>Returns all the direct and indirect cells that depend on this cell</returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            _dependencies.ReplaceDependees(name, null);
            if (text == "")
                _cells.Remove(name);
            else
                _cells[name] = new Cell(text);
            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        ///     Sets the contents of a cell to a formula object
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <param name="formula">Formula to be passed into the contents</param>
        /// <returns>Returns all the direct and indirect cells that depend on this cell</returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            _dependencies.ReplaceDependees(name, formula.GetVariables());
            try
            {
                var cells = GetCellsToRecalculate(name).ToList();
                _cells[name] = new Cell(formula);
                return cells;
            }
            catch (CircularException)
            {
                // Reverts ReplaceDependees since it's the only thing that changes before the error is thrown 
                _dependencies.ReplaceDependees(name,
                    GetCellContents(name) is Formula prevFormula ? prevFormula.GetVariables() : null);
                throw;
            }
        }

        /// <summary>
        ///     Returns all the cells that directly depends on the cell
        /// </summary>
        /// <param name="name">The cell to search for dependents</param>
        /// <returns></returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return _dependencies.GetDependents(name);
        }

        private class Cell
        {
            /// <summary>
            ///     Used to store contents and value of a cell.
            ///     Value can be updated when calling the evaluate method.
            /// </summary>
            /// <param name="contents">The information stored inside the cell</param>
            public Cell(object contents)
            {
                Contents = contents;
                Value = contents;
            }

            /// <summary>
            ///     Returns the calculate result of the contents in the cell.
            /// </summary>
            public object Value { get; private set; }

            /// <summary>
            ///     Returns the raw contents of the Cell
            /// </summary>
            public object Contents { get; }

            /// <summary>
            ///     Calculates and changes the value of the cell if the contents are a formula
            /// </summary>
            /// <param name="lookup">The method used to search for another cell's value</param>
            public void Evaluate(Func<string, double> lookup)
            {
                if (Contents is Formula formula) Value = formula.Evaluate(lookup);
            }
        }
    }
}