   M S B A M L   `   `   `  ����T  PAudioSteganographyProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]3clr-namespace:AudioSteganographyProject.UI.Controls%AudioSteganographyProject.UI.Controls  5      @    :AudioSteganographyProject.UI.Controls.FileSelectionControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Elocal3clr-namespace:AudioSteganographyProject.UI.Controls     5      .��5	      � $��5q�6     � RowDefinitions  5
   
   �� 5      $��30��6   $
��Auto �6+   �� 5      $��30��6   $��* �6+    � ColumnDefinitions 5   
   �� 5      -   5         �Name$
 col0��5      $	��250��6+   $
��Auto �6:   �� 5      $��5 �6   �� 5      $��40 �6   .+�5   
   �� -   5         ��Name$ descriptionTextBlock��5   +   $
��Wrap=�6I   $��0��6]   $��0��6m   $
��Left=�6z   $��Center=�6�   ��6   �� *Title+6�   �� 5   
   -   5         ��Name$ filePathInput��5   2   $��0��6I   $��1��6Y   $��Stretch=�6f   $��Center=�6�   ��6   �� *FilePathString+6�   �� 5   
   -   5        $��browseFilesButton��5       � ToolTip$ Browse��6+   $��2��6\   $��1��6l   .��5      (�  (� StretchDirection$ DownOnly=�6    (� Stretch$ Uniform=�62   .��5      �� $��Center=�6   Browse6A   5      5      5      5      