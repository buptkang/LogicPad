   M S B A M L   `   `   `  ����D  @LogicPad2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:LogicPad2	LogicPad2  5   	       LogicPad2.MainWindow   5   	   W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   8x,http://schemas.microsoft.com/winfx/2006/xaml    5   	   'localclr-namespace:LogicPad2    5   	   -   5          �Name$  logicPad2MainWindow��5   	    � Title$ 	LogicPad2��5   	    � WindowState$ 	Maximized=�6   .��5      � 
������� � RowDefinitions 5
   
   �� 5      $��22 �6    �� 5      $��30 �6    �� 5      $��* �6    .+�5   
   �� -   5         ��Name$ logicPadMenu��5      $��0��6'   ._�5      ��  �� Orientation$ 
Horizontal=�6   .+�5      o� $��22��6   $��0,0,0,0q�6'   
�������.@�5      m� $��_File��6#   $��14(�62   $��SemiBold"�6B   .@�5      m� -   5        $��E_xit��5   '   66   5      m� 5      $��_Representations��6#   $��14(�6=   $��SemiBold"�6M   .@�5      m� -   5         m�Name$ menuExpression��5   #   $��_Expression��6;   {�.  5      m� 5      -   5        $ menuDiagram��5   #   $��_Diagram��68   {�.  5      m� 5       -   5        $ menuTruthTable��5    #   $��_TruthTable��6;   {�.  5"      5#      m� 5&      $��_Options��6   $��14(�61   $��SemiBold"�6A   .@�5'      m� -   5        $��
_Recognize��5'   #   67   �� 5(      m� 5)      -   5        $��_Next��5)   #   63   �� 5*      m� 5+      -	   5        $��_UserId��5+   #   64   5,      m� 5.      $��_Mode��6   $��14(�6.   $��SemiBold"�6>   .@�5/      m� -
   5        $��practiceMenu��5/   6   $��	_Practice��6#   6J   �� 50      m� 51      -   5        $��testMenu��51   2   $��_Test��6#   6C   52      m� 55      $��
_Configure��6   $��14(�63   $��SemiBold"�6C   .@�56      m� -   5        $ touchOption��56   #   $��_Enabled Touch��68    m� 	IsChecked .  6b   57      m� 5:      $��_Help��6   $��14(�6.   $��SemiBold"�6=   .@�5;      m� -   5        $ about��5;   #   $��_About Logic Pad��62   6L   5<      5=      5>      5?      � 5A   
   $��1��6   $��Center=�6   .+�5B      �� -   5         ��Name$ sketchUserStudyPanel��5B   2   $ 
Horizontal=�6   $y�	Collapsed=�6P   .+�5C      �� -   5        	 ��Name$	 showingContent��5C      $
��Bold"�60   $��16(�6B   
���  ��6P   �� 5D      
���   �$��10,0,0,0q�6-   $��2q�6?   ._�5E      �� -   5        $ timer��5E   !   $ 
Horizontal=�61   $��Right=�6J   .+�5F      �� -   5        $��StartButton��5F   1   $��Start��6!   $��10,0,0,0q�6D   6V   �� 5G      -   5        $��
StopButton��5G   0   $
��Stop��6!   $��10,0,0,0q�6B   {�.  6m   5H      5I      5J      5K      �� 5M   
   $��2��6   
���   �$��4q�62   ._�5N      �  5O      �� 5P      $
��Auto �6$   �� 5Q      $��* �6$   .+�5U         LogicPad2.WidgetSelector  -   5        
  Name$
 widgetSelector��5U   '      LogicPad2.WidgetSelector   StylusEnter$ StylusEnterWidget��6X      StylusLeave$ StylusLeaveWidget��6x   $��0��6?   $��20��6L   6x   �� 5W      -   5         ��Name$ MainGCScroller��5W       $��1��68   $
��Auto=�5X       $
��Auto=�5Y       .��5[      #   LogicPad2.MSInkAnalysisCanvas  -   5          Name$ mainInkCanvas��5[   0   5\      �� 5^      -   5         ��Name$ adornerLayer��5^      $y�	Collapsed=�6/   5`      5a      5b      5c      