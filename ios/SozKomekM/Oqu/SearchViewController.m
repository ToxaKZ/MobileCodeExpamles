//
//  FirstViewController.m
//  Oqu
//
//  Created by Ельнар Шопанов on 21.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import "SearchViewController.h"
#import "DatabaseHelper.h"
#import "WordModel.h"
#import "ArticleViewController.h"
#import "ExceptionHelper.h"
#import "Constants.h"

@implementation SearchViewController

@synthesize wordsTableView;

- (void)viewDidLoad {
    [super viewDidLoad];

    [self setupSearchBar];
    
    UITapGestureRecognizer *tab = [[UITapGestureRecognizer alloc]initWithTarget:self action:@selector(dismissKeyboard)];
    tab.cancelsTouchesInView = NO;
    
    [self.view addGestureRecognizer:tab];
    [wordsTableView registerClass:[UITableViewCell self] forCellReuseIdentifier:IDENTIFIER_CELL];
    
    [self refresh];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
}

- (BOOL)searchBar:(UISearchBar *)searchBar shouldChangeTextInRange:(NSRange)range replacementText:(NSString *)text {
    return ([searchBar.text length] + [text length] - range.length <= 100);
}

- (void)searchBar:(UISearchBar *)searchBar textDidChange:(NSString *)searchText {
    [self refresh];
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return _dataSet != nil ? _dataSet.count : 0;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:IDENTIFIER_CELL forIndexPath:indexPath];
    
    WordModel *model = [_dataSet objectAtIndex:indexPath.row];
    cell.textLabel.text = model.name;
    
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    @try {
        WordModel *model = [_dataSet objectAtIndex:indexPath.row];
        
        if (model != nil) {
            [ArticleViewController showWithParent:self wordID:model.id wordName:model.name history:YES];
        }
    }
    @catch (NSException *e) {
        [[ExceptionHelper sharedInstance] alertWithException:e];
    }
}

- (void)scrollViewWillBeginDragging:(UIScrollView*)scrollView {
    [self dismissKeyboard];
}

- (void)refresh {
    if (_searchBar.text != nil && _searchBar.text.length > 0) {
        NSString *text = [[DatabaseHelper instance] prepareTextForSearch:_searchBar.text];
        _dataSet = [[DatabaseHelper instance] searchWordsByText:text];
    } else {
        _dataSet = nil;
    }
    
    [wordsTableView reloadData];
}

- (void)dismissKeyboard {
    [_searchBar resignFirstResponder];
}

- (void)setupSearchBar {
    _searchBar = [[UISearchBar alloc] init];
    _searchBar.placeholder = NSLocalizedString(RES_KEY_SEARCH, nil);
    _searchBar.delegate = self;
    
    self.navigationItem.titleView = _searchBar;
    
    _searchField = [self searchFieldWithView:_searchBar];
    
    if (_searchField != nil) {
        _searchField.returnKeyType = UIReturnKeyDone;
        _searchField.backgroundColor = [UIColor colorWithRed:220/255.0f green:220/255.0f blue:220/255.0f alpha:1.0f];
    }
    
    UIToolbar *toolBar = [[UIToolbar alloc] initWithFrame:CGRectMake(0, 0, self.view.bounds.size.width, 45)];
    toolBar.translucent = NO;
    toolBar.items = @[[[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemFlexibleSpace target:nil action:nil],
                      [[UIBarButtonItem alloc] initWithTitle:@"Ә" style:UIBarButtonItemStyleBordered target:self action:@selector(virtualButtonClick:)],
                      [[UIBarButtonItem alloc] initWithTitle:@"І" style:UIBarButtonItemStyleBordered target:self action:@selector(virtualButtonClick:)],
                      [[UIBarButtonItem alloc] initWithTitle:@"Ң" style:UIBarButtonItemStyleBordered target:self action:@selector(virtualButtonClick:)],
                      [[UIBarButtonItem alloc] initWithTitle:@"Ғ" style:UIBarButtonItemStyleBordered target:self action:@selector(virtualButtonClick:)],
                      [[UIBarButtonItem alloc] initWithTitle:@"Ү" style:UIBarButtonItemStyleBordered target:self action:@selector(virtualButtonClick:)],
                      [[UIBarButtonItem alloc] initWithTitle:@"Ұ" style:UIBarButtonItemStyleBordered target:self action:@selector(virtualButtonClick:)],
                      [[UIBarButtonItem alloc] initWithTitle:@"Қ" style:UIBarButtonItemStyleBordered target:self action:@selector(virtualButtonClick:)],
                      [[UIBarButtonItem alloc] initWithTitle:@"Ө" style:UIBarButtonItemStyleBordered target:self action:@selector(virtualButtonClick:)],
                      [[UIBarButtonItem alloc] initWithTitle:@"Һ" style:UIBarButtonItemStyleBordered target:self action:@selector(virtualButtonClick:)],
                      [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemFlexibleSpace target:nil action:nil]];
    _searchBar.inputAccessoryView = toolBar;
}

- (void)virtualButtonClick:(UIBarButtonItem*)sender {
    @try {
        if (_searchField != nil) {
            [_searchField replaceRange:_searchField.selectedTextRange withText:sender.title.lowercaseString];
        }
    }
    @catch (NSException *e) {
        [[ExceptionHelper sharedInstance] alertWithException:e];
    }
}

- (nullable UITextField*)searchFieldWithView:(UIView*)view {
    NSArray *subviews = view.subviews;
    
    if (subviews != nil && subviews.count > 0) {
        for (UIView *child in subviews) {
            if ([child isKindOfClass:[UITextField class]]) {
                return (UITextField*)child;
            } else {
                UITextField *field = [self searchFieldWithView:child];
                
                if (field != nil) {
                    return field;
                }
            }
        }
    }
    
    return nil;
}

@end