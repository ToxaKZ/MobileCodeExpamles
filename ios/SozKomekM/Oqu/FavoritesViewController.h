//
//  FavoritesViewController.h
//  Oqu
//
//  Created by Ельнар Шопанов on 07.03.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface FavoritesViewController : UIViewController {
    NSMutableArray *_dataSet;
}

@property (weak, nonatomic) IBOutlet UITableView *favoritesTableView;

- (void)refresh;

@end