using System;
using System.Diagnostics;
//using System.Drawing;
using System.Drawing.Printing;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using tetris2022;


namespace Tetris
{
    public partial class MainWindow : Window
    {
        public InputsManager input = new InputsManager();

        public MainWindow()
        {
            InitializeComponent();
            board = new Board(GameCanvas, FigureCanvas, score, level);

            overLabel.Visibility = Visibility.Hidden;

            createFigureBoard = new Board(AddFigure);
            board.scoreAdditonLabel = scoreAdd;
            previewBoard = new Board(FigurePreview);
            NewFigure = new Board(AddFigure);

            Color baseColor = Colors.Cyan;
            Color darkerCyan = Color.FromRgb(
                (byte)(baseColor.R * 0.9),
                (byte)(baseColor.G * 0.9),
                (byte)(baseColor.B * 0.9)
            );

            baseColor = Colors.Lime;
            Color darkerLime = Color.FromRgb(
                (byte)(baseColor.R * 0.9),
                (byte)(baseColor.G * 0.9),
                (byte)(baseColor.B * 0.9)
            );

            colors.Add(Colors.Red);
            colors.Add(darkerCyan);
            colors.Add(darkerLime);
            colors.Add(Colors.Blue);
            colors.Add(Colors.DarkOrange);
            colors.Add(Colors.Gold);
            colors.Add(Colors.DarkViolet);


            renderColors();
            


            List<Figures> pfigures = new List<Figures>();
            bool[,] coords;

            //tworzenie figur

            //Iblock
            coords = resetCoords();
            coords[1, 0] = true;
            coords[1, 1] = true;
            coords[1, 2] = true;
            coords[1, 3] = true;
            figureCoords.Add(coords);

            //Jblock
            coords = resetCoords();
            coords[1, 0] = true;
            coords[1, 1] = true;
            coords[1, 2] = true;
            coords[0, 2] = true;
            figureCoords.Add(coords);

            //Lblock
            coords = resetCoords();
            coords[1, 0] = true;
            coords[1, 1] = true;
            coords[1, 2] = true;
            coords[0, 2] = true;
            figureCoords.Add(coords);

            //Oblock
            coords = resetCoords();
            coords[0, 0] = true;
            coords[1, 0] = true;
            coords[1, 1] = true;
            coords[0, 1] = true;
            figureCoords.Add(coords);

            //Sblock
            coords = resetCoords();
            coords[2, 0] = true;
            coords[1, 0] = true;
            coords[1, 1] = true;
            coords[0, 1] = true;
            figureCoords.Add(coords);

            //Tblock
            coords = resetCoords();
            coords[1, 0] = true;
            coords[0, 1] = true;
            coords[1, 1] = true;
            coords[2, 1] = true;
            figureCoords.Add(coords);

            //Zblock
            coords = resetCoords();
            coords[0, 0] = true;
            coords[1, 0] = true;
            coords[1, 1] = true;
            coords[2, 1] = true;
            figureCoords.Add(coords);

            addFigure.IsHitTestVisible = false;
            AddColor.IsHitTestVisible = false;
            renderFigures();
        }
        Board previewBoard;
        Board createFigureBoard;
        Board board;
        Board NewFigure;
      

        public bool[,] resetCoords( )
        {
            
            return new bool[3, 4];
        }
        List<bool[,]> figureCoords = new List<bool[,]>();
        public async void startGame()
        {





            Random rand = new Random();

            int num1 = rand.Next(0, figureCoords.Count - 1);
            int num2 = rand.Next(0, figureCoords.Count - 1);
            Color color = randomColor();

            while (true)
            {
                await Task.Delay(100);



                Figures figure = new Figures();
                Figures pfigure = new Figures();



                //switch (num1)
                //{
                //    case 0: figure.IBlock(board, color, FigureCanvas); break;
                //    case 1: figure.JBlock(board, color, FigureCanvas); break;
                //    case 2: figure.LBlock(board, color, FigureCanvas); break;
                //    case 3: figure.OBlock(board, color, FigureCanvas); break;
                //    case 4: figure.SBlock(board, color, FigureCanvas); break;
                //    case 5: figure.TBlock(board, color, FigureCanvas); break;
                //    case 6: figure.ZBlock(board, color, FigureCanvas); break;
                //}
                while (num1 > figureCoords.Count - 1)
                {
                    num1--;
                }
                figure.NewFigure(board, color, FigureCanvas, figureCoords[num1], true);

                //lista wspolrzednych ktore potem rzutuje na newfigure z losowymi wspolrzendymi!!!!!!!!

                currentFigure = new Figures();
                currentFigure = figure;
                board.currentFigure = currentFigure;
                color = randomColor();
                while (num2 > figureCoords.Count - 1)
                {
                    num2--;
                }
                if (!currentFigure.gameOver)
                {
                    pfigure.NewFigure(board, color, FigurePreview, figureCoords[num2], true);
                    //skalowanie na previewboard
                    scaleToPreview(pfigure);
                }
                    

                await currentFigure.BlockFall(board);


                if (currentFigure.gameOver)
                {
                    FigurePreview.Children.Clear();
                    

                    previewBoard = new Board(FigurePreview);
                    LevelAfter.Content = "Level: " + board.levelInt;
                    ScoreAfter.Content = board.score;
                    overLabel.Visibility = Visibility.Visible;
                    start.Content = "Restart";

                    return;

                }

                if (!currentFigure.isFalling)
                {
                    num1 = num2;
                    num2 = rand.Next(0, 7);

                }
                if (pfigure.FigureBlocks != null)
                {
                    foreach (Block block in pfigure.FigureBlocks)
                    {
                        FigurePreview.Children.Remove(block.rectangle);
                    }
                }
                
            } 

        }
        public void resetGame()
        {
            GameCanvas.Children.Clear();
            board = new Board(GameCanvas, FigureCanvas, score, level);

            overLabel.Visibility = Visibility.Hidden;
            FigurePreview.Children.Clear();
            createFigureBoard = new Board(AddFigure);
            board.scoreAdditonLabel = scoreAdd;
            previewBoard = new Board(FigurePreview);
            NewFigure = new Board(AddFigure);

            board.score = 0;
            board.levelInt = 1;

            startGame();
        }

        public void scaleToPreview(  Figures pfigure)
        {
            int previewWidth = 3;
            int previewHeight = 4;

            int offsetX = (previewWidth - pfigure.width) / 2 - pfigure.minX;
            int offsetY = (previewHeight - pfigure.height) / 2 - pfigure.minY;
            if (currentFigure.gameOver)
            {
                return;
            }
            if (pfigure.FigureBlocks != null)
            {
                foreach (Block block in pfigure.FigureBlocks)
                {

                    block.x += offsetX;
                    block.y += offsetY;
                    block.UpdateVisual();
                }
            }

            
            
        }
   

        Figures currentFigure = new Figures();

        Figures nextFigure = new Figures();
        //kontrolki na klawiaturze
        private void EKeyDown(object sender, KeyEventArgs e)
        {
            if (currentFigure == null || currentFigure.FigureBlocks == null || currentFigure.isFalling == false)
                return;

            if (awaitLeftKey || awaitRightKey || awaitRotateKey || awaitFallKey)
                return;

            if (currentFigure == null)
                return;

            if (!currentFigure.ispaused)
            {
                if (e.Key == leftKey)
                {
                    if (currentFigure.isFalling)
                    {

                        currentFigure.goLeft(input);

                    }

                }
                if (e.Key == rightKey)
                {
                    if (currentFigure.isFalling)
                    {
                        currentFigure.goRight(input);
                    }
                }
                if (e.Key == rotateKey)
                {

                    currentFigure.rotate();

                }
                if (e.Key == fallKey)
                {

                    currentFigure.changeDelay(10);

                }
            }
            
        }


        int i = 0;
        bool stopGame = false;

        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (i == 0)
            {
                startGame();
            }

            if (i % 2 == 0)
            {
                start.Content = "PAUSE";
                stopGame = true;
                currentFigure.ispaused = false;
                i++;
            }
            else
            {
                start.Content = "RESUME";
                currentFigure.ispaused = true;
                i++;
            }

            if (currentFigure.gameOver)
            {
                resetGame();
                i+=2;
            }


        }
        List<Color> colors = new List<Color>();
        public Color randomColor()
        {
            if (colors.Count == 0)
            {
                Random rand = new Random();
                byte R = (byte)rand.Next(0, 255);
                byte G = (byte)rand.Next(0, 255);
                byte B = (byte)rand.Next(0, 255);
                return Color.FromRgb(R, G, B);

            }
            else
            {
                Random rand = new Random();
                bool found = false;
                int index;
                do
                {
                    index = rand.Next(0, colors.Count);
                    for (int i = 0; i < 3; i++)
                    {
                        if (lastColors[i] == colors[index])
                        {
                            found = true;
                            break;
                        }
                        else
                        {
                            found = false;
                        }
                    }
                } while (found && colors.Count > 3);

                //fifo
                lastColors[2] = lastColors[1];
                lastColors[1] = lastColors[0];
                lastColors[0] = colors[index];

                return colors[index];
            }
            
            

        }
        Color[] lastColors = new Color[3];

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            TabControl.SelectedIndex = 1;
        }


        public Border defaultBorder(int width, int height) {
            Border border = new Border();
            border.Width = width;
            border.Height = height;
            border.BorderThickness = new Thickness(2.5);
            border.BorderBrush = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#004bb5"),
                                                      (Color)ColorConverter.ConvertFromString("#2071aa"), 45);
            return border;
        }

        public Grid createGrid(int width, int height, Color background)
        {
            Grid grid = new Grid();
            grid.Width = width;
            grid.Height = height;
            grid.Background = new SolidColorBrush(background);
            return grid;
        }
        List<Border> figureUiList = new List<Border>();
        List<Border> colorsUiList = new List<Border>();
        
        public void renderColors()
        {
            
            foreach(Border border in colorsUiList)
            {
                colorsList.Children.Remove(border);
            }

            foreach (Color color in colors)
            {

                
                Border border = defaultBorder(75,105);
                border.Margin = new Thickness(5);

                Grid outerGrid = createGrid(70, 95, (Color)ColorConverter.ConvertFromString("#000a2e"));
                
                Grid colorGrid = createGrid(40,40 , color);
                colorGrid.VerticalAlignment = VerticalAlignment.Top;
                colorGrid.Margin = new Thickness(0,10,0,0);

                Button button = new Button();
                button.Margin = new Thickness(0, 0, 0, 5);
                button.Content = "Delete";
                button.FontSize = 17;
                button.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./res/#Jersey 10"); ;
                button.Height = 30;
                button.Width = 50;
                button.VerticalAlignment = VerticalAlignment.Bottom;
                button.BorderBrush = Brushes.Red;
                button.BorderThickness = new Thickness(2.5);
                button.Background = Brushes.Transparent;
                button.Foreground = Brushes.Red;
                button.Padding = new Thickness(0);

                outerGrid.Children.Add(colorGrid);
                outerGrid.Children.Add(button);
                border.Child = outerGrid;
                colorsUiList.Add(border);
                colorsList.Children.Add(border);

                button.Click += (sender, e) =>
                {
                    colors.Remove(color);
                    colorsList.Children.Remove(border);

                };
                button.MouseEnter += DeleteBtnEnter;
                button.MouseLeave += DeleteBtnLeave;

            }
        }


        private void BackBtnClicked(object sender, RoutedEventArgs e)
        {
            TabControl.SelectedIndex = 0;
        }

        Color NewColor = new Color();

        private void ColorPicker(object sender, RoutedEventArgs e)
        {
            Window1 colorPicker = new Window1();
            bool? result = colorPicker.ShowDialog();
            if (result == true) 
            {
                AddColor.IsHitTestVisible = true;
                colorBtn.Background = new SolidColorBrush(colorPicker.color);
                colorBtn.Background = new SolidColorBrush(colorPicker.color);
                NewColor = colorPicker.color;
            }
            else
            {
                AddColor.IsHitTestVisible = false;
            }
        }

        private void AddNewColor(object sender, RoutedEventArgs e)
        {
            colors.Add(NewColor);
            renderColors();
        }

        bool[,] newFigureCoords;

        private void FigurePicker(object sender, RoutedEventArgs e)
        {
            AddFigure.Children.Clear();
            NewFigure = new Board(AddFigure);
            FigurePicker figurePicker = new FigurePicker(colors);
            bool? result = figurePicker.ShowDialog();
            if (result == true)
            {
                bool[,] newFigure = figurePicker.coordList;
                Figures figure = new Figures();
                figure.NewFigure(NewFigure, figurePicker.color, AddFigure, newFigure, false);
                newFigureCoords = newFigure;
                addFigure.IsHitTestVisible = true;
            }
            else
            {
                addFigure.IsHitTestVisible = false;
                
            }
            

        }

        private void colorsTab(object sender, RoutedEventArgs e)
        {
            SettingsControl.SelectedIndex = 0;
        }

        private void figuresTab(object sender, RoutedEventArgs e)
        {
            SettingsControl.SelectedIndex = 1;
         
        }
        private void controlsTab(object sender, RoutedEventArgs e)
        {
            SettingsControl.SelectedIndex = 2;
        }

        public void renderFigures()
        {


            foreach (Border border in figureUiList)
            {
                figureList.Children.Remove(border);
            }

            foreach (bool[,] coords in figureCoords)
            {
                

                Border border = defaultBorder(86, 135);
                border.Margin = new Thickness(5);

                Grid outerGrid = createGrid(81, 130, (Color)ColorConverter.ConvertFromString("#000a2e"));

                Border innerBorder = defaultBorder(50, 65);
                innerBorder.Margin = new Thickness(0, -25, 0, 0);



                Canvas canvas = new Canvas();
                canvas.Width = 45;
                canvas.Height = 60;
                
                NewFigure = new Board(canvas);
                Figures figure = new Figures();
                figure.NewFigure(NewFigure, randomColor(), canvas, coords,false);
                
                Button button = new Button();
                button.Margin = new Thickness(0, 0, 0, 6);
                button.Content = "Delete";
                button.FontSize = 17;
                button.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./res/#Jersey 10"); ;
                button.Height = 30;
                button.Width = 50;
                button.VerticalAlignment = VerticalAlignment.Bottom;
                button.BorderBrush = Brushes.Red;
                button.BorderThickness = new Thickness(2.5);
                button.Background = Brushes.Transparent;
                button.Foreground = Brushes.Red;


                button.Padding = new Thickness(0);



                outerGrid.Children.Add(innerBorder);
                innerBorder.Child = canvas;
                outerGrid.Children.Add(button);
                border.Child = outerGrid;
                figureUiList.Add(border);
                figureList.Children.Add(border);

                button.Click += (sender, e) =>
                {
                    figureCoords.Remove(coords);
                    figureList.Children.Remove(border);

                };
                button.MouseEnter += DeleteBtnEnter;
                button.MouseLeave += DeleteBtnLeave;

            }


        
        }

        private void AddNewFigure(object sender, RoutedEventArgs e)
        {



                figureCoords.Add(newFigureCoords);
                renderFigures();

                
        }

        private void AddBtnEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush( Colors.LimeGreen);
            button.Foreground = new SolidColorBrush(Colors.White);
        }

        private void AddBtnLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.Foreground = new SolidColorBrush(Colors.LimeGreen);
        }

        private void DeleteBtnEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            button.Background = new SolidColorBrush(Colors.Red);
            button.Foreground = new SolidColorBrush(Colors.White);
        }

        private void DeleteBtnLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.Foreground = new SolidColorBrush(Colors.Red);
        }
        private void DefaultBtnEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new System.Windows.Point(0, 0);
            brush.EndPoint = new System.Windows.Point(1, 1);

            brush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(0, 75, 181), 0));
            brush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(32, 113, 170), 0.5));
            brush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(0, 75, 181), 1));

            


            button.Foreground = brush;
        }

        private void DefaultBtnLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            button.Foreground = new SolidColorBrush(Colors.White);
        }
        private void NavBtnEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            button.Foreground = new SolidColorBrush(Color.FromRgb(180,180,180));
        }

        private void NavBtnLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            button.Foreground = new SolidColorBrush(Colors.White);
        }

        private void BackBtnEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            backArr.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            backBtn.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180));
        }

        private void BackBtnLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            backArr.Foreground = new SolidColorBrush(Colors.White);
            backBtn.Foreground = new SolidColorBrush(Colors.White);
        }

        bool awaitLeftKey = false;
        Key leftKey = Key.A;
        bool awaitRightKey = false;
        Key rightKey = Key.D;
        bool awaitRotateKey = false;
        Key rotateKey = Key.W;
        bool awaitFallKey = false;
        Key fallKey = Key.S;
        private void AwaitLeftKey(object sender, RoutedEventArgs e)
        {
            awaitLeftKey = true;

        }

        private void SetLeftKey(object sender, KeyEventArgs e)
        {
            if (awaitLeftKey)
            {
                leftKey = e.Key;
                awaitLeftKey = false;
                leftKeyLabel.Content = e.Key.ToString();
            }
        }
        private void AwaitRightKey(object sender, RoutedEventArgs e)
        {
            awaitRightKey = true;
        }

        private void SetRightKey(object sender, KeyEventArgs e)
        {
            if (awaitRightKey)
            {
                rightKey = e.Key;
                awaitRightKey = false;
                rightKeyLabel.Content = e.Key.ToString();
            }
        }
        private void AwaitRotateKey(object sender, RoutedEventArgs e)
        {
            awaitRotateKey= true;
        }

        private void SetRotateKey(object sender, KeyEventArgs e)
        {
            if (awaitRotateKey)
            {
                rotateKey = e.Key;
                awaitRotateKey = false;
                rotateKeyLabel.Content = e.Key.ToString();
            }
        }
        private void AwaitFallKey(object sender, RoutedEventArgs e)
        {
            awaitFallKey = true;
        }

        private void SetFallKey(object sender, KeyEventArgs e)
        {
            if (awaitFallKey)
            {
                fallKey = e.Key;
                awaitFallKey = false;
                fallKeyLabel.Content = e.Key.ToString();
            }
        }


    }



    public class InputsManager
    {
        public bool aPressed, dPressed;

    }
    public class Figures
    {
        public bool isFalling = false;
        public Block[] FigureBlocks = new Block[4];
        public Block[] GhostFigure = new Block[4];
        public int height = 0;
        public int width = 0;

        public Board board;
        public bool gameOver = false;
        public Canvas canvas;
        public Canvas GameCanvas;
        public int delay = 500;

        public int minY;
        public int maxY;
        public int minX;
        public int maxX;

        public int rotations = 2;
        public bool ispaused = false;
        public int size = 4;

        public void changeDelay(int delay)
        {
            this.delay = delay;
        }

        public void rotate()
        {
            if (FigureBlocks == null || FigureBlocks.Length == 0)
                return;
            int sumX = 0;
            int sumY = 0;

            int[] oldXT = new int[FigureBlocks.Length];
            int[] oldYT = new int[FigureBlocks.Length];

            for (int i = 0; i < FigureBlocks.Length; i++)
            {
                oldXT[i] = FigureBlocks[i].x;
                oldYT[i] = FigureBlocks[i].y;
            }

            int moveLeft = 0;
            int moveRight = 0;
            int moveDown = 0;
            bool possible = true;

            foreach (Block block in FigureBlocks)
            {
                sumX += block.x;
                sumY += block.y;

            }

            int pivotX = sumX / FigureBlocks.Length;
            int pivotY = sumY / FigureBlocks.Length;


            foreach (Block block in FigureBlocks)
            {
                int oldX = block.x - pivotX;
                int oldY = block.y - pivotY;

                int newX = -oldY;
                int newY = oldX;

                if (this.width != 3)
                {
                    if (rotations % 2 == 0)
                    {
                        block.x = pivotX + newX + 1;
                    }
                    else
                    {
                        block.x = pivotX + newX;
                    }
                }
                else
                {
                    block.x = pivotX + newX;
                }



                block.y = pivotY + newY;

                if (block.y < 0)
                {
                    moveDown = -1 * block.y;
                }

                block.y = pivotY + newY;

                if (block.x < 0)
                {
                    moveRight = -1 * block.x;
                }
                else if (block.x > 9)
                {
                    moveLeft = block.x - 9;
                }
                else
                {
                    block.UpdateVisual();
                }


                if (block.x == 10)
                {
                    moveLeft++;
                }


            }

            foreach (Block block in FigureBlocks)
            {

                block.x += moveRight - moveLeft;
                block.y += moveDown;
                if (block.x >= 0 && !board.gameBoard[block.x, block.y].isFree)
                {
                    possible = false;
                }


            }
            if (!possible)
            {
                int i = 0;
                foreach (Block block in FigureBlocks)
                {
                    block.x = oldXT[i];
                    block.y = oldYT[i];
                    i++;
                }
            }



            foreach (Block block in FigureBlocks)
            {
                block.UpdateVisual();
            }

            refreshGhost();
            rotations++;
        }

        public int linearMap(int number, int size)
        {
            if (size % 2 == 0)
            {
                int a = 0;
                int b = size;
                int c = size / 2 * -1;
                int d = size / 2;

                return (number - a) * ((d - c) / (b - a)) + c;

            }
            return 0;
        }

        public Color ghostColor(Color color, byte alpha)
        {

            return Color.FromArgb(alpha, color.R, color.G, color.B);


        }

        public void deleteGhost()
        {
            for (int i = 0; i < GhostFigure.Length; i++)
            {
                GhostFigure[i].x = 0;
                GhostFigure[i].y = 0;
                GhostFigure[i].color = Colors.Transparent;
            }
        }

        public void refreshGhost()
        { 
            if(this.gameOver == true)
            {
                return;
            }
            for (int i = 0; i < GhostFigure.Length; i++)
            {
                if (GhostFigure != null)
                {
                    GhostFigure[i].x = FigureBlocks[i].x;
                    GhostFigure[i].y = FigureBlocks[i].y;
                    GhostFigure[i].color = ghostColor(FigureBlocks[i].color, 75);
                }
            }
            bool canFall = true;
            //petla spadania

            while (canFall)
            {
                canFall = true;
                foreach (Block ghostBlock in GhostFigure)
                {
                    if (ghostBlock.y + 1 >= 20 || (ghostBlock.x >= 0 && !board.gameBoard[ghostBlock.x, ghostBlock.y + 1].isFree))
                    {
                        canFall = false;
                        break;
                    }
                }
                if (canFall)
                {
                    foreach (Block ghostBlock in GhostFigure)
                        ghostBlock.y++;
                }
            }

            foreach (Block ghostBlock in GhostFigure)
                ghostBlock.UpdateVisual();


            canFall = true;
        }

        public void figureGhost()
        {

            GhostFigure = new Block[FigureBlocks.Length];

            for (int i = 0; i < FigureBlocks.Length; i++)
            {
                GhostFigure[i] = new Block(FigureBlocks[i].x, FigureBlocks[i].y, true, ghostColor(FigureBlocks[i].color, 100), canvas);
            }

            refreshGhost();

        }

        public void NewFigure(Board board, Color color, Canvas canvas, bool[,] coords, bool isForGameBoard)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (coords[x, y])
                    {
                        int boardX = isForGameBoard ? x + 4 : x; // pozycja X na planszy
                        int boardY = y;

                        // jeśli sprawdzamy planszę główną i blok jest zajęty → nie tworzymy figury
                        if (isForGameBoard && !board.gameBoard[boardX, boardY].isFree)
                        {
                            this.FigureBlocks = null; // figura nie istnieje
                            this.gameOver = true; // opcjonalnie, można też ustawić gameOver
                            return;
                        }
                    }
                }
            }
            size = 0;
            int iteration = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 4; y++)
                {
    
                    if (coords[x, y])
                    {
                        size++;
                    }
 
                }
            }

            FigureBlocks = new Block[size];
            if (isForGameBoard)
            {
                
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        if (coords[x, y])
                        {

                            FigureBlocks[iteration] = new Block(x + 4, y, false, color, canvas);
                            iteration++;
                        }

                    }
                }
            }
            else
            {
                int maxY = 0;

                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        if (coords[x, y])
                            maxY = Math.Max(maxY, y);
                    }
                }
                int figureFall = 0;
                for (int y = 0; y < 4; y++)
                {
                    if ((coords[0, y] == false && coords[1, y] == false && coords[2, y] == false)&&y>maxY)
                    {
                        figureFall++;
                    }

                }
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        if (coords[x, y])
                        {
                            FigureBlocks[iteration] = new Block(x , y + figureFall, false, color, canvas);
                            iteration++;
                        }

                    }
                }
            }


                this.height = GetHeight();
            this.width = GetWidth();
            this.canvas = canvas;
        }

        public async Task BlockFall(Board board)
        {
            if (this.gameOver)
            {
                this.isFalling = false;
                return;
            }
            board.checkRow();
            this.board = board;
            figureGhost();

            //Figures f = new Figures();
            //f.Figure(board, Colors.Red, FigureCanvas);


            //kontrolki do bondow


            bool canFall = true;


            //petla spadania

            this.isFalling = true;
            while (true)
            {
                await Task.Delay(this.delay);
                if (!ispaused)
                {

                    foreach (Block block in FigureBlocks)
                    {
                        if ((block.y + 1 >= 20 || block.x < 0) ||
                            !board.gameBoard[block.x, block.y + 1].isFree)
                        {
                            canFall = false;
                            deleteGhost();
                            break;
                        }
                    }

                    if (!canFall)
                        break;

                    foreach (Block block in FigureBlocks)
                    {
                        block.y++;
                        block.UpdateVisual();
                    }
                }


            }

            this.isFalling = false;


            if (!this.isFalling)
            {
                foreach (Block block in this.FigureBlocks)
                {
                    block.isFree = false;
                    Block boardBlock = board.gameBoard[block.x, block.y];
                    boardBlock.isFree = false;
                    boardBlock.color = block.color;
                    boardBlock.rectangle.Fill = new LinearGradientBrush(
                        Color.FromArgb(block.color.A, (byte)Math.Min(block.color.R + 180, 255), (byte)Math.Min(block.color.G + 180, 255), (byte)Math.Min(block.color.B + 180, 255)), block.color, 45);
                    boardBlock.rectangle.Stroke = new LinearGradientBrush(
                        block.color,
                        Color.FromArgb(block.color.A, (byte)(block.color.R * 0.8), (byte)(block.color.G * 0.8), (byte)(block.color.B * 0.8)), 45);
                    boardBlock.rectangle.StrokeThickness = 1;

                    if (block.y == 0)
                    {
                        this.gameOver = true;
                    }
                }
            }

            canFall = true;
            canvas.Children.Clear();
            board.score += FigureBlocks.Length;


        }

        public void goLeft(InputsManager input)
        {
            bool ispossible = true;

            foreach (Block block in this.FigureBlocks)
            {
                if (block.x - 1 < 0)
                {
                    ispossible = false;
                }
                else

                if (board.gameBoard[block.x - 1, block.y].isFree == false)
                {
                    ispossible = false;
                }
            }
            if (ispossible)
            {

                foreach (Block block in this.FigureBlocks)
                {
                    block.x--;
                    block.UpdateVisual();
                }
                refreshGhost();

            }
        }

        public void goRight(InputsManager input)
        {
            bool ispossible = true;
            foreach (Block block in this.FigureBlocks)
            {
                if (block.x + 1 > 9)
                {
                    ispossible = false;
                }
                else if (board.gameBoard[block.x + 1, block.y].isFree == false)
                {
                    ispossible = false;
                }
            }
            if (ispossible)
            {

                foreach (Block block in this.FigureBlocks)
                {
                    block.x++;
                    block.UpdateVisual();
                }
                refreshGhost();

            }
        }

        private int GetHeight()
        {
            int i = 0;

            int[] sortedYIndex = new int[this.size];

            foreach (Block block in this.FigureBlocks)
            {
                sortedYIndex[i] = FigureBlocks[i].y;
                i++;
            }
            Array.Sort(sortedYIndex);

            int YMax = sortedYIndex[FigureBlocks.Length - 1];
            int Ymin = sortedYIndex[0];
            this.maxY = YMax;
            this.minY = Ymin;
            return YMax - Ymin + 1;
        }

        private int GetWidth()
        {
            int i = 0;

            int[] sortedXIndex = new int[size];

            foreach (Block block in this.FigureBlocks)
            {
                sortedXIndex[i] = FigureBlocks[i].x;
                i++;
            }
            Array.Sort(sortedXIndex);

            int XMax = sortedXIndex[FigureBlocks.Length - 1];
            int Xmin = sortedXIndex[0];
            this.maxX = XMax;
            this.minX = Xmin;
            return XMax - Xmin;
        }


      


    }

    public class Block
    {
        public int x, y;
        public bool isFree = true;
        public Color color;
        public Rectangle rectangle;
        public Stroke stroke;
        public int strokeThickness;
        public Canvas canvas;
        public Block(int x, int y, bool isFree, Color color)
        {
            this.x = x;
            this.y = y;
            this.isFree = isFree;
            this.color = color;
        }

        public Block(int x, int y, bool isFree)
        {
            this.x = x;
            this.y = y;
            this.isFree = isFree;
        }

        //bloki dla figur na nowym canvasie
        public Block(int x, int y, bool isFree, Color color, Canvas canvas)
        {
            this.x = x;
            this.y = y;
            this.color = color;

            rectangle = new Rectangle
            {
                Width = 15,
                Height = 15,
                Fill = new LinearGradientBrush(
                    Color.FromArgb(color.A, (byte)Math.Min(this.color.R + 180, 255), (byte)Math.Min(this.color.G + 180, 255), (byte)Math.Min(this.color.B + 180, 255)),
                    Color.FromArgb(color.A, this.color.R, this.color.G, this.color.B),
                    45
                    ),
                Stroke = new LinearGradientBrush(
                    Color.FromArgb(color.A, this.color.R, this.color.G, this.color.B),
                    Color.FromArgb(color.A, (byte)(this.color.R * 0.8), (byte)(this.color.G * 0.8), (byte)(this.color.B * 0.8)),
                    45
                    ),
                StrokeThickness = 1
            };
            Canvas.SetLeft(rectangle, x * 15);
            Canvas.SetTop(rectangle, y * 15);
            canvas.Children.Add(rectangle);

        }



        public void MoveDown()
        {
            y++;
            Canvas.SetTop(rectangle, y * 15);
        }


        public void UpdateVisual()
        {
            if (rectangle != null)
            {

                Canvas.SetLeft(rectangle, this.x * 15);
                Canvas.SetTop(rectangle, this.y * 15);
                Canvas.SetZIndex(rectangle, 1);
            }

        }

    }
    public class Board
    {
        public Block[,] gameBoard = new Block[10, 20];
        public Block[,] previewBoard = new Block[3, 4];
        public Canvas GameCanvas;
        public Canvas FigureCanvas;
        public System.Windows.Controls.Label scorelabel;
        public System.Windows.Controls.Label level;
        public System.Windows.Controls.Label scoreAdditonLabel;
        public int score = 0;
        public Figures currentFigure;
        public int levelInt = 1;

        public async Task deleteRows(List<int> fullRows)
        {

            foreach (int row in fullRows)
            {
                await Task.Delay(300);

                for (int y2 = row; y2 > 0; y2--)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        Block block = gameBoard[x, y2];
                        Block above = gameBoard[x, y2 - 1];

                        if (!above.isFree)
                        {
                            block.isFree = false;
                            block.color = above.color;
                            block.rectangle.Fill = new LinearGradientBrush(
                                Color.FromArgb(above.color.A, (byte)Math.Min(above.color.R + 180, 255),
                                                          (byte)Math.Min(above.color.G + 180, 255),
                                                          (byte)Math.Min(above.color.B + 180, 255)),
                                above.color,
                                45);
                            block.rectangle.Stroke = new LinearGradientBrush(
                                above.color,
                                Color.FromArgb(above.color.A,
                                               (byte)(above.color.R * 0.8),
                                               (byte)(above.color.G * 0.8),
                                               (byte)(above.color.B * 0.8)),
                                45);
                            block.rectangle.StrokeThickness = 1;
                        }
                        else
                        {
                            block.isFree = true;
                            block.color = Colors.Transparent;
                            block.rectangle.Fill = new SolidColorBrush(Color.FromRgb(2, 22, 40));
                            block.rectangle.Stroke = new SolidColorBrush(Color.FromRgb(20, 51, 110));
                            block.rectangle.StrokeThickness = 0.5;
                        }

                        block.UpdateVisual();
                    }
                }

            }
        }
        public async Task checkRow()
        {
            if (currentFigure.gameOver)
            {
                return;
            }
            int delay = 500 - levelInt * 20;

            if (delay < 200)
            {
                delay = 200;
            }
            currentFigure.delay = delay;
            List<int> fullRows = new List<int>();

            for (int y = 0; y < 20; y++)
            {
                bool fullRow = true;
                for (int x = 0; x < 10; x++)
                {
                    if (gameBoard[x, y].isFree)
                    {
                        fullRow = false;
                        break;
                    }
                }
                if (fullRow)
                    fullRows.Add(y);
            }


            foreach (int y in fullRows)
            {
                for (int x = 0; x < 10; x++)
                {
                    gameBoard[x, y].isFree = true;
                    gameBoard[x, y].color = Colors.Transparent;
                    gameBoard[x, y].rectangle.Fill = new SolidColorBrush(Color.FromRgb(2, 22, 40));
                    gameBoard[x, y].rectangle.Stroke = new SolidColorBrush(Color.FromRgb(20, 51, 110));
                    gameBoard[x, y].rectangle.StrokeThickness = 0.5;
                    
                }
            }

            deleteRows(fullRows);
            
            int prevscore = score;
            foreach (int y in fullRows)
            {

                score += 30 + fullRows.Count * 10;

            }

            scoreAdditonLabel.Foreground = new SolidColorBrush(Color.FromRgb(190, 190, 190));
            scoreAdditonLabel.FontSize = 20;
            scoreAdditonLabel.Height = 30;
            scorelabel.Margin = new Thickness(0, 0, -5, 0);

            if (score != 0)
            {
                if ((score - prevscore + currentFigure.FigureBlocks.Length) > 4)
                {
                    scoreAdditonLabel.Foreground = Brushes.LimeGreen;
                    scoreAdditonLabel.FontSize = 25;
                    scoreAdditonLabel.Height = 32.5;

                }
                scoreAdditonLabel.Content = "+" + (score - prevscore + currentFigure.FigureBlocks.Length);
                await Task.Delay(1000);
                currentFigure.refreshGhost();

                scoreAdditonLabel.Content = "";
            }

            scorelabel.Margin = new Thickness(0, 0, -15, 0);
            scorelabel.Content = score;



            if (score > 100 * levelInt)
            {
                levelInt = (score / 100) + 1;
                level.Content = "Level: " + levelInt;
                await FlashLevel(5);

            }
            if (currentFigure.delay > 200)
            {
                currentFigure.delay = 500 - levelInt * 20;
            }


        }

        public async Task FlashLevel(int times)
        {
            for (int i = 0; i < times; i++)
            {
                level.Visibility = Visibility.Hidden;
                await Task.Delay(200);
                level.Visibility = Visibility.Visible;
                await Task.Delay(200);
            }
        }

        public Board(Canvas canvas)
        {
            this.GameCanvas = canvas;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    //tworzenie bloku
                    Rectangle rectangle = new Rectangle
                    {
                        Width = 15,
                        Height = 15,
                        Fill = new SolidColorBrush(Color.FromRgb(2, 22, 40)),
                        Stroke = new SolidColorBrush(Color.FromRgb(20, 51, 110)),
                        StrokeThickness = 0.5

                    };

                    Canvas.SetLeft(rectangle, x * 15);
                    Canvas.SetTop(rectangle, y * 15);

                    GameCanvas.Children.Add(rectangle);

                    Block block = new Block(x, y, true);

                    block.rectangle = rectangle;

                    // tworzenie planszy jako liste blokow
                    previewBoard[x, y] = block;


                }
            }
        }

        public Board(Canvas canvas, Canvas figureCanvas, System.Windows.Controls.Label scorelabel, System.Windows.Controls.Label level)
        {
            this.GameCanvas = canvas;//przekazanie canvasu bo inaczej nie dziala
            this.scorelabel = scorelabel;
            //petla po planszy
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    //tworzenie wizualnego bloku
                    Rectangle rectangle = new Rectangle
                    {
                        Width = 15,
                        Height = 15,
                        Fill = new SolidColorBrush(Color.FromRgb(2, 22, 40)),
                        Stroke = new SolidColorBrush(Color.FromRgb(20, 51, 110)),
                        StrokeThickness = 0.5

                    };
                    Canvas.SetLeft(rectangle, x * 15);
                    Canvas.SetTop(rectangle, y * 15);
                    GameCanvas.Children.Add(rectangle);

                    Block block = new Block(x, y, true);

                    block.rectangle = rectangle;

                    // tworzenie planszy jako liste blokow
                    gameBoard[x, y] = block;


                }
            }

            FigureCanvas = figureCanvas;
            this.level = level;
        }
    }
}