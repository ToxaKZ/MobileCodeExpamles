//
//  DatabaseHelper.m
//  Oqu
//
//  Created by Ельнар Шопанов on 27.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import "DatabaseHelper.h"
#import "WordModel.h"
#import "ArticleModel.h"
#import "NSData+GZIP.h"
#import "FavoriteModel.h"
#import "HistoryModel.h"

@implementation DatabaseHelper

NSString *const AVAILABLE_LETTERS = @"абвгдёежзийклмнопрстуфхцчшщъыьэюяәіңғүұқөһqwertyuiopasdfghjklzxcvbnm";
int const MAX_RESULT_SET = 100;
NSString *const KEY_HISTORY = @"history";
NSString *const KEY_FAVORITES = @"favorites";

static DatabaseHelper *helper = nil;

+ (DatabaseHelper*)instance {
    if (helper == nil) {
        helper = [[DatabaseHelper alloc] init];
    }
    
    return helper;
}

- (id)init {
    if ((self = [super init])) {
//        NSFileManager *fileManager = [NSFileManager defaultManager];
//        NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
//        _fileName = [[paths objectAtIndex:0] stringByAppendingPathComponent:@"store.sqlite"];
//        
//        if (![fileManager fileExistsAtPath:_fileName] || [self needCopyDatabase]) {
//            NSString *dbPath = [[NSBundle mainBundle] pathForResource:@"store" ofType:@"sqlite3"];
//            NSError *error;
//            [fileManager copyItemAtPath:dbPath toPath:_fileName error:&error];
//        }
        
        _availableTableNames = [NSMutableArray array];
        sqlite3_open([[[NSBundle mainBundle] pathForResource:@"store" ofType:@"sqlite3"] UTF8String], &_db);
        
        [self setAvailableTableNames];
    }
    
    return self;
}

- (void)dealloc {
    sqlite3_close(_db);
}

- (NSString*)tableNameByText:(NSString*)text {
    NSString *result = @"words";
    
    if (text != nil && [text length] > 0) {
        NSRange range = [AVAILABLE_LETTERS rangeOfString:[text substringWithRange:NSMakeRange(0, 1)]];
        
        if (range.location != NSNotFound) {
            result = [NSString stringWithFormat:@"%@_%d", result, (int)range.location];
        }
    }
    
    return result;
}

- (NSArray*)searchWordsByText:(nonnull NSString*)text {
    NSString *tableName = [self tableNameByText:text];
    
    if ([_availableTableNames containsObject:tableName]) {
        NSString *query = [NSString stringWithFormat:@"select w.word_id, w.name from %@ w where w.name like ? order by w.name limit %d", tableName, MAX_RESULT_SET];
        sqlite3_stmt *stmt;
        
        if (sqlite3_prepare_v2(_db, [query UTF8String], -1, &stmt, nil) == SQLITE_OK) {
            @try {
                if (sqlite3_bind_text(stmt, 1, [[text stringByAppendingString:@"%"] UTF8String], -1, SQLITE_TRANSIENT) != SQLITE_OK) {
                    return nil;
                }
                
                NSMutableArray *result = [[NSMutableArray alloc] init];
                
                while (sqlite3_step(stmt) == SQLITE_ROW) {
                    WordModel *model = [[WordModel alloc] initWithId:sqlite3_column_int(stmt, 0) name:[[NSString alloc] initWithUTF8String:(char *) sqlite3_column_text(stmt, 1)]];
                    [result addObject:model];
                }
                
                return result;
            }
            @finally {
                sqlite3_finalize(stmt);
            }
        }
    }
    
    return nil;
}

- (void)setAvailableTableNames {
    NSString *query = @"select m.name from sqlite_master m where m.type = ?";
    sqlite3_stmt *stmt;
    
    if (sqlite3_prepare_v2(_db, [query UTF8String], -1, &stmt, nil) == SQLITE_OK) {
        @try {
            if (sqlite3_bind_text(stmt, 1, [@"table" UTF8String], -1, SQLITE_TRANSIENT) != SQLITE_OK) {
                return;
            }
            
            while (sqlite3_step(stmt) == SQLITE_ROW) {
                [_availableTableNames addObject:[[NSString alloc] initWithUTF8String:(char *) sqlite3_column_text(stmt, 0)]];
            }
        }
        @finally {
            sqlite3_finalize(stmt);
        }
    }
}

- (NSString*)prepareTextForSearch:(nonnull NSString*)text {
    NSCharacterSet *cs = [NSCharacterSet characterSetWithCharactersInString:@"'_?%"];
    return [[[text lowercaseString] componentsSeparatedByCharactersInSet:cs] componentsJoinedByString: @""];
}

- (nullable NSMutableArray*)history {
    NSUserDefaults *prefs = [NSUserDefaults standardUserDefaults];
    NSMutableArray *dataArray = [prefs objectForKey:KEY_HISTORY];
    
    if (dataArray != nil) {
        NSMutableArray *array = [NSMutableArray arrayWithCapacity:dataArray.count];
        
        for (NSData *child in dataArray) {
            [array addObject:[NSKeyedUnarchiver unarchiveObjectWithData:child]];
        }
        
        return array;
    }
    
    return nil;
}

- (void)insertHistory:(nonnull HistoryModel*)history {
    [self deleteHistory:history];
    
    NSMutableArray *array = [self history];
    
    if (array == nil) {
        array = [NSMutableArray array];
    }
    
    [array insertObject:history atIndex:0];
    [self saveHistory:array];
}

- (void)deleteHistory:(nonnull HistoryModel*)history {
    NSMutableArray *array = [self history];
    
    if (array != nil) {
        NSInteger count = array.count;
        
        for (int i = 0; i < count; i++) {
            HistoryModel *model = [array objectAtIndex:i];
            
            if (history.wordId == model.wordId) {
                [array removeObjectAtIndex:i];
                break;
            }
        }
        
        [self saveHistory:array];
    }
}

- (void)clearHistory {
    NSMutableArray *array = [self history];
    
    if (array != nil) {
        [array removeAllObjects];
        [self saveHistory:array];
    }
}

- (void)saveHistory:(nonnull NSMutableArray*)array {
    NSMutableArray *dataArray = [NSMutableArray arrayWithCapacity:array.count];
    
    for (HistoryModel *model in array) {
        [dataArray addObject:[NSKeyedArchiver archivedDataWithRootObject:model]];
    }
    
    NSUserDefaults *prefs = [NSUserDefaults standardUserDefaults];
    [prefs setObject:dataArray forKey:KEY_HISTORY];
    [prefs synchronize];
}

//- (NSMutableArray*)history {
//    NSString *query = [NSString stringWithFormat:@"select h.id, h.name, h.date_create, h.word_id from history h order by h.date_create desc limit %d", MAX_RESULT_SET];
//    sqlite3_stmt *stmt;
//
//    if (sqlite3_prepare_v2(_db, [query UTF8String], -1, &stmt, nil) == SQLITE_OK) {
//        @try {
//            NSMutableArray *result = [NSMutableArray array];
//
//            while (sqlite3_step(stmt) == SQLITE_ROW) {
//                HistoryModel *model = [[HistoryModel alloc] initWithId:sqlite3_column_int(stmt, 0)
//                                                                  name:[[NSString alloc] initWithUTF8String:(char *) sqlite3_column_text(stmt, 1)]
//                                                            dateCreate:[NSDate dateWithTimeIntervalSince1970:sqlite3_column_int(stmt, 2)]
//                                                                wordId:sqlite3_column_int(stmt, 3)];
//                [result addObject:model];
//            }
//
//            return result;
//        }
//        @finally {
//            sqlite3_finalize(stmt);
//        }
//    }
//
//    return nil;
//}

//- (void)insertHistoryWithWordId:(int)wordId wordName:(NSString*)wordName {
//    NSString *query = @"insert or replace into history (word_id, name, date_create) values (?, ?, ?)";
//    
//    sqlite3_stmt *stmt;
//    
//    if (sqlite3_prepare_v2(_db, [query UTF8String], -1, &stmt, nil) == SQLITE_OK) {
//        @try {
//            sqlite3_bind_int(stmt, 1, wordId);
//            sqlite3_bind_text(stmt, 2, [wordName UTF8String], -1, SQLITE_TRANSIENT);
//            sqlite3_bind_int(stmt, 3, [[NSDate date] timeIntervalSince1970]);
//            sqlite3_step(stmt);
//        }
//        @finally {
//            sqlite3_finalize(stmt);
//        }
//    }
//}

//- (void)deleteHistoryWIthId:(int)id {
//    NSString *query = @"delete from history where id = ?";
//    sqlite3_stmt *stmt;
//
//    if (sqlite3_prepare_v2(_db, [query UTF8String], -1, &stmt, nil) == SQLITE_OK) {
//        @try {
//            sqlite3_bind_int(stmt, 1, id);
//            sqlite3_step(stmt);
//        }
//        @finally {
//            sqlite3_finalize(stmt);
//        }
//    }
//}

//- (void)clearHistory {
//    NSString *query = @"delete from history";
//    sqlite3_stmt *stmt;
//
//    if (sqlite3_prepare_v2(_db, [query UTF8String], -1, &stmt, nil) == SQLITE_OK) {
//        @try {
//            sqlite3_step(stmt);
//        }
//        @finally {
//            sqlite3_finalize(stmt);
//        }
//    }
//}

- (int)wordIdWithText:(NSString*)text {
    NSString *tableName = [self tableNameByText:text];
    
    if ([_availableTableNames containsObject:tableName]) {
        NSString *query = [NSString stringWithFormat:@"select w.word_id from %@ w where w.name = ?", tableName];
        sqlite3_stmt *stmt;
        
        if (sqlite3_prepare_v2(_db, [query UTF8String], -1, &stmt, nil) == SQLITE_OK) {
            @try {
                sqlite3_bind_text(stmt, 1, [text UTF8String], -1, SQLITE_TRANSIENT);
                
                if (sqlite3_step(stmt) == SQLITE_ROW) {
                    return sqlite3_column_int(stmt, 0);
                }
            }
            @finally {
                sqlite3_finalize(stmt);
            }
        }
    }
    
    return -1;
}

//- (BOOL)needCopyDatabase {
//    sqlite3 *db;
//    
//    sqlite3_open([_fileName UTF8String], &db);
//    @try {
//        NSString *query = @"select count(*) from sqlite_master m where m.type = ? and m.name = ?";
//        sqlite3_stmt *stmt;
//        
//        if (sqlite3_prepare_v2(db, [query UTF8String], -1, &stmt, nil) == SQLITE_OK) {
//            @try {
//                sqlite3_bind_text(stmt, 1, [@"table" UTF8String], -1, SQLITE_TRANSIENT);
//                sqlite3_bind_text(stmt, 2, [@"settings" UTF8String], -1, SQLITE_TRANSIENT);
//                
//                if (sqlite3_step(stmt) == SQLITE_ROW && sqlite3_column_int(stmt, 0) > 0) {
//                    sqlite3_stmt *sc;
//                    
//                    if (sqlite3_prepare_v2(db, [@"select max(v.version) from settings v" UTF8String], -1, &sc, nil) == SQLITE_OK) {
//                        @try {
//                            if (sqlite3_step(sc) == SQLITE_ROW && DB_VERSION == sqlite3_column_int(sc, 0)) {
//                                return NO;
//                            }
//                        }
//                        @finally {
//                            sqlite3_finalize(sc);
//                        }
//                    }
//                }
//            }
//            @finally {
//                sqlite3_finalize(stmt);
//            }
//        }
//    }
//    @finally {
//        sqlite3_close(db);
//    }
//    
//    return YES;
//}

- (nullable NSMutableArray*)favorites {
    NSUserDefaults *prefs = [NSUserDefaults standardUserDefaults];
    NSMutableArray *dataArray = [prefs objectForKey:KEY_FAVORITES];
    
    if (dataArray != nil) {
        NSMutableArray *array = [NSMutableArray arrayWithCapacity:dataArray.count];
        
        for (NSData *child in dataArray) {
            [array addObject:[NSKeyedUnarchiver unarchiveObjectWithData:child]];
        }
        
        return array;
    }
    
    return nil;
}

- (void)insertFavorite:(nonnull FavoriteModel*)favorite {
    [self deleteFavorite:favorite];
    
    NSMutableArray *array = [self favorites];
    
    if (array == nil) {
        array = [NSMutableArray array];
    }
    
    [array addObject:favorite];
    [self saveFavorites:array];
}

- (void)deleteFavorite:(nonnull FavoriteModel*)favorite {
    NSMutableArray *array = [self favorites];
    
    if (array != nil) {
        NSInteger count = array.count;
        
        for (int i = 0; i < count; i++) {
            FavoriteModel *model = [array objectAtIndex:i];
            
            if (favorite.wordId == model.wordId) {
                [array removeObjectAtIndex:i];
                break;
            }
        }
        
        [self saveFavorites:array];
    }
}

- (void)clearFavorites {
    NSMutableArray *array = [self favorites];
    
    if (array != nil) {
        [array removeAllObjects];
        [self saveFavorites:array];
    }
}

- (void)saveFavorites:(nonnull NSMutableArray*)array {
    NSMutableArray *dataArray = [NSMutableArray arrayWithCapacity:array.count];
    
    for (FavoriteModel *model in array) {
        [dataArray addObject:[NSKeyedArchiver archivedDataWithRootObject:model]];
    }
    
    NSUserDefaults *prefs = [NSUserDefaults standardUserDefaults];
    [prefs setObject:dataArray forKey:KEY_FAVORITES];
    [prefs synchronize];
}

//- (NSMutableArray*)favorites {
//    NSString *query = [NSString stringWithFormat:@"select f.id, f.name, f.word_id from favorites f order by f.name limit %d", MAX_RESULT_SET];
//    sqlite3_stmt *stmt;
//    
//    if (sqlite3_prepare_v2(_db, [query UTF8String], -1, &stmt, nil) == SQLITE_OK) {
//        @try {
//            NSMutableArray *result = [NSMutableArray array];
//            
//            while (sqlite3_step(stmt) == SQLITE_ROW) {
//                FavoriteModel *model = [[FavoriteModel alloc] initWithId:sqlite3_column_int(stmt, 0)
//                                                                    name:[[NSString alloc] initWithUTF8String:(char *) sqlite3_column_text(stmt, 1)]
//                                                                  wordId:sqlite3_column_int(stmt, 2)];
//                [result addObject:model];
//            }
//            
//            return result;
//        }
//        @finally {
//            sqlite3_finalize(stmt);
//        }
//    }
//    
//    return nil;
//}

//- (void)insertFavoriteWithWordId:(int)wordId wordName:(NSString*)wordName {
//    NSString *query = @"insert or replace into favorites (word_id, name) values (?, ?)";
//    sqlite3_stmt *stmt;
//
//    if (sqlite3_prepare_v2(_db, [query UTF8String], -1, &stmt, nil) == SQLITE_OK) {
//        @try {
//            sqlite3_bind_int(stmt, 1, wordId);
//            sqlite3_bind_text(stmt, 2, [wordName UTF8String], -1, SQLITE_TRANSIENT);
//            sqlite3_step(stmt);
//        }
//        @finally {
//            sqlite3_finalize(stmt);
//        }
//    }
//}

//- (void)deleteFavoriteWIthId:(int)id {
//    NSString *query = @"delete from favorites where id = ?";
//    sqlite3_stmt *stmt;
//    
//    if (sqlite3_prepare_v2(_db, [query UTF8String], -1, &stmt, nil) == SQLITE_OK) {
//        @try {
//            sqlite3_bind_int(stmt, 1, id);
//            sqlite3_step(stmt);
//        }
//        @finally {
//            sqlite3_finalize(stmt);
//        }
//    }
//}

//- (void)clearFavorites {
//    NSString *query = @"delete from favorites";
//    sqlite3_stmt *stmt;
//    
//    if (sqlite3_prepare_v2(_db, [query UTF8String], -1, &stmt, nil) == SQLITE_OK) {
//        @try {
//            sqlite3_step(stmt);
//        }
//        @finally {
//            sqlite3_finalize(stmt);
//        }
//    }
//}

@end