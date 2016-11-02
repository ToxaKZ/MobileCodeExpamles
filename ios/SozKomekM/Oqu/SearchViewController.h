//
//  FirstViewController.h
//  Oqu
//
//  Created by Ельнар Шопанов on 21.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SearchViewController : UIViewController <UISearchBarDelegate, UITableViewDelegate> {
    NSArray *_dataSet;
    UISearchBar *_searchBar;
    UITextField *_searchField;
}

@property (weak, nonatomic) IBOutlet UITableView *wordsTableView;

@end