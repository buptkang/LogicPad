   M S B A M L   `   `   `  ���� D  @LogicPad2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:LogicPad2	LogicPad2      LogicPad2.MainWindow   R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    'localclr-namespace:LogicPad2    -     �Name$  logicPad2MainWindow�� � Title$ 	LogicPad2�� � WindowState$ 	Maximized=�.��� 
������� � RowDefinitions �� $��22 ��� $��30 ��� $��* �.+��� -    ��Name$ logicPadMenu��$��0��._���  �� Orientation$ 
Horizontal=�.+�o� $��22��$��0,0,0,0q�
�������.@�m� $��_File��$��14(�$��SemiBold"�.@�m� -   $��E_xit��m� $��_Representations��$��14(�$��SemiBold"�.@�m� -    m�Name$ menuExpression��$��_Expression��{�.  m� -   $ menuDiagram��$��_Diagram��{�.  m� -   $ menuTruthTable��$��_TruthTable��{�.  m� $��_Options��$��14(�$��SemiBold"�.@�m� -   $��
_Recognize���� m� -   $��_Next���� m� -	   $��_UserId��m� $��_Mode��$��14(�$��SemiBold"�.@�m� -
   $��practiceMenu��$��	_Practice���� m� -   $��testMenu��$��_Test��m� -   $ 
toutchMenu��$��
_Configure��$��14(�$��SemiBold"�.@�m� -   $ touchOption��$��_Enabled Touch�� m� 	IsChecked .  m� $��_Help��$��14(�$��SemiBold"�.@�m� -   $ about��$��_About Logic Pad��� $��1��$��Center=�.+��� -    ��Name$ sketchUserStudyPanel��$ 
Horizontal=�$y�	Collapsed=�.+��� -   	 ��Name$	 showingContent��$
��Bold"�$��16(�
���  ���� 
���   �$��10,0,0,0q�$��2q�._��� -   $ timer��$ 
Horizontal=�$��Right=�.+��� -   $��StartButton��$��Start��$��10,0,0,0q��� -   $��
StopButton��$
��Stop��$��10,0,0,0q�{�.  �� $��2��
���   �$��4q�._��  �� $
��Auto ��� $��* �.+�   LogicPad2.WidgetSelector  -   
  Name$
 widgetSelector��   LogicPad2.WidgetSelector   StylusEnter$ StylusEnterWidget��   StylusLeave$ StylusLeaveWidget��$��0��$��20���� -    ��Name$ MainGCScroller��$��1��$
��Auto=�$
��Auto=�.��#   LogicPad2.MSInkAnalysisCanvas  -     Name$ mainInkCanvas���� -    ��Name$ adornerLayer��$y�	Collapsed=�